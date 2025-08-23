using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.Diagnostics;
using Nemonuri.Failures;
using OllamaSharp;

namespace Nemonuri.OllamaRunning;

public static partial class OllamaRunningTheory
{
    public static async
    Task<GetOllamaServerVersionResult>
    GetOllamaServerVersionAsync
    (
        Uri? serverUri = null,
        CancellationToken cancellationToken = default
    )
    {
        const string methodLabel = $"[{nameof(GetOllamaServerVersionAsync)}]";

        Uri ensuredServerUri = serverUri ?? OllamaRunningConstants.DefaultOllamaServerUri;
        OllamaApiClient? ollamaApiClient = null;
        StrongBox<bool> timeOutedBox = new(false);
        TimeSpan timeLimit = TimeSpan.FromSeconds(10);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            ollamaApiClient = new(ensuredServerUri);
            Debug.WriteLine($"{methodLabel} {nameof(OllamaApiClient)} created. Uri = {ollamaApiClient.Uri}");

            //--- Request version to given ollama server URI ---
            using CancellationTokenSource timerCts = new();
            var timerCt = timerCts.Token;
            timerCt.UnsafeRegister
            (
                static obj =>
                {
                    if (obj is StrongBox<bool> v1) { v1.Value = true; }
                },
                timeOutedBox
            );
            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timerCt);
            timerCts.CancelAfter(timeLimit);

            Debug.WriteLine($"{methodLabel} Run {nameof(OllamaApiClient)}.{nameof(ollamaApiClient.GetVersionAsync)}. TimeLimit = {timeLimit}");
            string versionResponse = await ollamaApiClient.GetVersionAsync(combinedCts.Token).ConfigureAwait(false);
            Debug.WriteLine($"{methodLabel} Response = {versionResponse}");

            if (CheckSourceHasCorrectGetVersionResponse(versionResponse))
            {
                string msg = "Response is valid.";
                Debug.WriteLine($"{methodLabel} {msg}");
                return GetOllamaServerVersionResult.CreateAsValue((ollamaApiClient, versionResponse));
            }
            else
            {
                string msg = "Response is invalid.";
                Debug.WriteLine($"{methodLabel} {msg}");
                return GetOllamaServerVersionResult.CreateAsInvalidResponse(versionResponse, msg);
            }
        }
        catch (OperationCanceledException e)
        {
            ollamaApiClient?.Dispose();
            // Check canceled is raised from timeout.
            Debug.WriteLine($"{methodLabel} {e.Message}");
            if (timeOutedBox.Value)
            {
                return GetOllamaServerVersionResult.CreateAsTimeOut(timeLimit, e.Message);
            }
            else
            {
                return GetOllamaServerVersionResult.CreateAsFailure(GetOllamaServerVersionResult.FailInfo.Canceled, e.Message);
            }
        }
        catch (HttpRequestException e) when (e.InnerException is SocketException se)
        {
            ollamaApiClient?.Dispose();
            Debug.WriteLine($"{methodLabel} {e.Message}");
            return GetOllamaServerVersionResult.CreateAsSocketError(se.ErrorCode, e.Message);
        }
        catch (SocketException e)
        {
            ollamaApiClient?.Dispose();
            Debug.WriteLine($"{methodLabel} {e.Message}");
            return GetOllamaServerVersionResult.CreateAsSocketError(e.ErrorCode, e.Message);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"{methodLabel}{Environment.NewLine}{e}");
            ollamaApiClient?.Dispose();
            throw;
        }
    }


    public static async
    Task<GetClientAfterEnsuringOllamaServerRunningResult>
    GetClientAfterEnsuringOllamaServerRunningAsync
    (
        Uri? serverUri = null,
        bool enableRunningLocalOllamaServer = false,
        string localOllamaHostCommand = OllamaRunningConstants.DefaultLocalOllamaHostCommand,
        CancellationToken cancellationToken = default
    )
    {
        const string methodLabel = $"[{nameof(GetClientAfterEnsuringOllamaServerRunningAsync)}]";
        //const string returnErrorLabel = "[Error]";

        OllamaApiClient? ollamaApiClient = null;
        OllamaLocalServerProcess? ollamaLocalServerProcess = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            //--- Request version to given ollama server URI ---
            Debug.WriteLine($"{methodLabel} Run {nameof(GetOllamaServerVersionAsync)}. {nameof(serverUri)} = {serverUri}");
            GetOllamaServerVersionResult getOllamaServerVersionResult = await GetOllamaServerVersionAsync(serverUri, cancellationToken).ConfigureAwait(false);
            if (getOllamaServerVersionResult.IsFailure)
            {
                const string msg1 = $"{nameof(GetOllamaServerVersionAsync)} Failed.";
                Debug.WriteLine($"{methodLabel} {msg1}");
                Debug.WriteLine($"{methodLabel} {nameof(getOllamaServerVersionResult)} = {getOllamaServerVersionResult}");

                //--- Return error, if version request failed and running local ollama server disabled ---
                if (!enableRunningLocalOllamaServer)
                {
                    const string msg2 = "And running local ollama server disabled.";
                    Debug.WriteLine($"{methodLabel} {msg2}");
                    const string msg3 = $"{msg1} {msg2}";

                    return GetClientAfterEnsuringOllamaServerRunningResult.CreateAsGetOllamaServerVersionFailed
                    (
                        getOllamaServerVersionResult.GetFailInfo(),
                        $"{msg3}{Environment.NewLine}{getOllamaServerVersionResult.GetMessage()}"
                    );
                }
                //---|

                //--- Get local Ollama server running state ---
                Debug.WriteLine($"{methodLabel} Run {nameof(GetLocalOllamaServerRunningStateAsync)}. {nameof(localOllamaHostCommand)} = {localOllamaHostCommand}");
                GetLocalOllamaServerRunningStateResult getLocalOllamaServerRunningStateResult =
                    await GetLocalOllamaServerRunningStateAsync(localOllamaHostCommand, cancellationToken).ConfigureAwait(false);
                if (getLocalOllamaServerRunningStateResult.IsFailure)
                {
                    const string msg4 = $"{nameof(GetLocalOllamaServerRunningStateAsync)} Failed.";
                    Debug.WriteLine($"{methodLabel} {msg4}");
                    Debug.WriteLine($"{methodLabel} {nameof(getLocalOllamaServerRunningStateResult)} = {getLocalOllamaServerRunningStateResult}");

                    return GetClientAfterEnsuringOllamaServerRunningResult.CreateAsGetLocalOllamaServerRunningStateFailed
                    (
                        getLocalOllamaServerRunningStateResult.GetFailInfo(),
                        $"{msg4}{Environment.NewLine}{getOllamaServerVersionResult.GetMessage()}"
                    );
                }
                else if (getLocalOllamaServerRunningStateResult.GetValue() == LocalOllamaServerRunningState.Running)
                {
                    return GetClientAfterEnsuringOllamaServerRunningResult.CreateAsFailure
                    (
                        GetClientAfterEnsuringOllamaServerRunningResult.FailInfo.LocalOllamaServerIsAlreadyRunning
                    );
                }
                //---|

                Guard.IsTrue(getLocalOllamaServerRunningStateResult.GetValue() == LocalOllamaServerRunningState.Idle);

                //--- Run local ollama server ---
                var runLocalOllamaServerAsyncResult = await RunLocalOllamaServerAsync(localOllamaHostCommand, cancellationToken).ConfigureAwait(false);
                if (runLocalOllamaServerAsyncResult.IsFailure)
                {
                    return GetClientAfterEnsuringOllamaServerRunningResult.CreateAsRunLocalOllamaServerFailed
                    (
                        runLocalOllamaServerAsyncResult.GetFailInfo(),
                        runLocalOllamaServerAsyncResult.GetMessage()
                    );
                }

                ollamaLocalServerProcess = runLocalOllamaServerAsyncResult.GetValue();
                //---|

                //--- Request version to given ollama server URI, again ---
                GetOllamaServerVersionResult getOllamaServerVersionResult2 = await GetOllamaServerVersionAsync(serverUri, cancellationToken).ConfigureAwait(false);
                if (getOllamaServerVersionResult2.IsFailure)
                {
                    return GetClientAfterEnsuringOllamaServerRunningResult.CreateAsGetOllamaServerVersionAgainFailed
                    (
                        getOllamaServerVersionResult2.GetFailInfo(),
                        getOllamaServerVersionResult2.GetMessage()
                    );
                }
                ollamaApiClient = getOllamaServerVersionResult2.GetValue().Item1;
                //---| 
            }
            else
            {
                Debug.Assert(getOllamaServerVersionResult.IsValue);
                (ollamaApiClient, var receivedVersionString) = getOllamaServerVersionResult.GetValue();

                Debug.WriteLine($"{methodLabel} {nameof(receivedVersionString)} = {receivedVersionString}");
            }
            //---|

            //--- Return Ollama api client ---
            return GetClientAfterEnsuringOllamaServerRunningResult.CreateAsValue((ollamaApiClient, ollamaLocalServerProcess));
            //---|

        }
        catch (OperationCanceledException e)
        {
            ollamaApiClient?.Dispose();
            ollamaLocalServerProcess?.Dispose();
            Debug.WriteLine($"{methodLabel} {e.Message}");
            return GetClientAfterEnsuringOllamaServerRunningResult.CreateAsFailure
            (
                GetClientAfterEnsuringOllamaServerRunningResult.FailInfo.Canceled,
                e.Message
            );
        }
        catch (Exception e)
        {
            ollamaApiClient?.Dispose();
            ollamaLocalServerProcess?.Dispose();
            Debug.WriteLine($"{methodLabel}{Environment.NewLine}{e}");
            throw;
        }

#if false
        [Conditional("DEBUG")]
        static void DebugWriteLine(string subLabel, string message) =>
            Debug.WriteLine($"{methodLabel}{subLabel} {message}");
#endif
    }

    public static async
    Task<GetLocalOllamaServerRunningStateResult>
    GetLocalOllamaServerRunningStateAsync
    (
        string localOllamaHostCommand = OllamaRunningConstants.DefaultLocalOllamaHostCommand,
        CancellationToken cancellationToken = default
    )
    {
        const string methodLabel = $"[{nameof(GetLocalOllamaServerRunningStateAsync)}]";

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            //--- Run local ollama server version check command ---
            using Process process = new();
            process.StartInfo = new()
            {
                FileName = localOllamaHostCommand,
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            StringBuilder errorStringBuilder = new();
            process.ErrorDataReceived += ErrorDataReceived_Handle;

            void ErrorDataReceived_Handle(object sender, DataReceivedEventArgs e)
            {
                if (e.Data is { } data)
                {
                    errorStringBuilder.AppendLine(data);
                }
            }

            Debug.WriteLine
            (
                $"""
                {methodLabel} Starting local ollama host process. 
                {nameof(process)} = {process}
                """
            );

            var startOrFailResult = process.StartOrFail();
            if (startOrFailResult.IsFailure)
            {
                Debug.WriteLine($"{methodLabel} {startOrFailResult.GetMessage()}");
                return GetLocalOllamaServerRunningStateResult.CreateAsProcessStartFailed
                (
                    value: startOrFailResult.GetFailInfo(),
                    message: startOrFailResult.GetMessage()
                );
            }
            //---|

            //--- get local ollama server version checking process result ---
            process.BeginErrorReadLine();

            char[] stdoutBlock = new char[256];
            int stdoutBlockEnsuredLength = await process.StandardOutput.ReadBlockAsync(stdoutBlock, cancellationToken).ConfigureAwait(false);

            process.Close();

            string errorString = errorStringBuilder.ToString();
            string stdoutString = new(stdoutBlock, 0, stdoutBlockEnsuredLength);

            Debug.WriteLine
            (
                $"""
                [{nameof(GetLocalOllamaServerRunningStateAsync)}] Process closed. 
                {nameof(errorString)} = {errorString}
                {nameof(stdoutString)} = {stdoutString}
                """
            );
            //---|

            //--- Solve local ollama server running state ---
            if (CheckSourceHasCorrectGetVersionResponse(stdoutString))
            {
                Debug.WriteLine
                (
                    $"""
                    {methodLabel} local ollama server running state solved. 
                    result = {LocalOllamaServerRunningState.Running}
                    """
                );
                return GetLocalOllamaServerRunningStateResult.CreateAsValue(LocalOllamaServerRunningState.Running);
            }
            else if
            (
                stdoutString.Contains("could not connect") &&
                stdoutString.Contains("client version is")
            )
            {
                Debug.WriteLine
                (
                    $"""
                    {methodLabel} local ollama server running state solved. 
                    result = {LocalOllamaServerRunningState.Idle}
                    """
                );
                return GetLocalOllamaServerRunningStateResult.CreateAsValue(LocalOllamaServerRunningState.Running);
            }
            else
            {
                Debug.WriteLine
                (
                    $"""
                    {methodLabel} Cannot solve local ollama server running state. 
                    result = {errorString}
                    """
                );
                return GetLocalOllamaServerRunningStateResult.CreateAsFailure(GetLocalOllamaServerRunningStateResult.FailInfo.SolveLocalOllamaServerRunningStateFailed, errorString);
            }
            //---|
        }
        catch (OperationCanceledException e)
        {
            return GetLocalOllamaServerRunningStateResult.CreateAsFailure(GetLocalOllamaServerRunningStateResult.FailInfo.Canceled, e.Message);
        }
    }

    private static bool CheckSourceHasCorrectGetVersionResponse(string source) =>
        !string.IsNullOrWhiteSpace(source) &&
        source.Contains("ollama version is");

    public static async Task<RunLocalOllamaServerResult>
    RunLocalOllamaServerAsync
    (
        string localOllamaHostCommand = OllamaRunningConstants.DefaultLocalOllamaHostCommand,
        CancellationToken cancellationToken = default
    )
    {
        const string methodLabel = $"[{nameof(RunLocalOllamaServerAsync)}]";
        
        Process? process = null;
        OllamaLocalServerStartInfo? ollamaLocalServerStartInfo = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            //--- Start new local ollama server process ---
            //const string subLabel1 = "Start new local ollama server process";

            process = new();
            process.StartInfo = new()
            {
                FileName = localOllamaHostCommand,
                Arguments = "serve",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using ManualResetEvent observingLocalOllamaServerCompletedSignal = new(initialState: false);

            //--- subscribe ErrorDataReceived event ---
            ConcurrentQueue<string> errorStringQueue = new();
            void ErrorDataReceived_Handle(object sender, DataReceivedEventArgs e)
            {
                if (e.Data is { } data)
                {
                    errorStringQueue.Enqueue(data);
                    observingLocalOllamaServerCompletedSignal.Set();
                }
            }

            process.ErrorDataReceived += ErrorDataReceived_Handle;
            //---|

            //--- subscribe ErrorDataReceived event ---
            ConcurrentQueue<string> outputStringQueue = new();
            void OutputDataReceived_Handle(object sender, DataReceivedEventArgs e)
            {
                if (e.Data is { } data)
                {
                    outputStringQueue.Enqueue(data);
                    var match = GetOllamaServerLogListeningOnRegex().Match(data.Trim());
                    if (match.Success)
                    {
                        ollamaLocalServerStartInfo = new OllamaLocalServerStartInfo
                        (
                            ListenerAddress: match.Groups["ListenerAddress"].Value,
                            Version: match.Groups["Version"].Value
                        );
                        observingLocalOllamaServerCompletedSignal.Set();
                    }
                }
            }

            process.OutputDataReceived += OutputDataReceived_Handle;
            //---|

            ProcessStartingTheory.StartOrFailResult processStartResult = process.StartOrFail();
            if (processStartResult.IsFailure)
            {
                string msg = "Failed to start process.";
                Debug.WriteLine($"{methodLabel} {msg}");
                return RunLocalOllamaServerResult.CreateAsProcessStartFailed
                (
                    processStartResult.GetFailInfo(),
                    $"{msg} {processStartResult.GetMessage()}"
                );
            }
            //---|

            //--- Observe local Ollama server successfully started ---
            const string subLabel2 = "Observe local Ollama server successfully started";
            Debug.WriteLine($"{methodLabel} {subLabel2}");
            TimeSpan timeSpan = TimeSpan.FromSeconds(10);
            using CancellationTokenSource timeOutCts = new(timeSpan);

            int signaledWaitHandleIndex = await Task.Run(() =>
            {
                return WaitHandle.WaitAny([cancellationToken.WaitHandle, timeOutCts.Token.WaitHandle, observingLocalOllamaServerCompletedSignal]);
            }, cancellationToken);

            //--- unsubscribe events ---
            process.ErrorDataReceived -= ErrorDataReceived_Handle;
            process.OutputDataReceived -= OutputDataReceived_Handle;
            //---|

            cancellationToken.ThrowIfCancellationRequested();

            if (timeOutCts.IsCancellationRequested)
            {
                Debug.Assert(signaledWaitHandleIndex == 1);
                return RunLocalOllamaServerResult.CreateAsFailure(RunLocalOllamaServerResult.FailInfo.TimeOut, $"TimeOut. {nameof(timeSpan)} = {timeSpan}");
            }

            Debug.Assert(signaledWaitHandleIndex == 2);

            if (!errorStringQueue.IsEmpty)
            {
                return RunLocalOllamaServerResult.CreateAsFailure
                (
                    RunLocalOllamaServerResult.FailInfo.ErrorReceivedFromLocalServer,
                    string.Join(Environment.NewLine, errorStringQueue)
                );
            }
            
            Debug.WriteLine($"{methodLabel} Confirm local Ollama server successfully started.");
            Debug.Assert(ollamaLocalServerStartInfo is not null);

            return RunLocalOllamaServerResult.CreateAsValue(new OllamaLocalServerProcess(
                process, ollamaLocalServerStartInfo.ListenerAddress, ollamaLocalServerStartInfo.Version
            ));
            //---|
        }
        catch (OperationCanceledException e)
        {
            process?.Dispose();
            Debug.WriteLine($"{methodLabel} {e.Message}");
            return RunLocalOllamaServerResult.CreateAsFailure(RunLocalOllamaServerResult.FailInfo.Canceled, e.Message);
        }
        catch (Exception e)
        {
            process?.Dispose();
            Debug.WriteLine
            (
                $"""
                {methodLabel}
                {e}
                """
            );
            throw;
        }

#if false
        [Conditional("DEBUG")]
        static void DebugWriteLine(string subLabel, string message) =>
            Debug.WriteLine($"[{nameof(RunLocalOllamaServerAsync)}]{subLabel} {message}");
#endif
    }

    [GeneratedRegex(
        """
        time=(?<Time>\S*)\s+level=(?<Level>\S*)\s+source=(?<Source>\S*)\s+msg="Listening on (?<ListenerAddress>\S*)\s+\(version (?<Version>\S*?)\)"
        """,
        RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    internal static partial Regex GetOllamaServerLogListeningOnRegex();
}


