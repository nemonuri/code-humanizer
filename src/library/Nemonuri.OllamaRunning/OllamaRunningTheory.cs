using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
                return GetOllamaServerVersionResult.CreateAsFailure(GetOllamaServerVersionResult.FailInfo.Cancel, e.Message);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"{methodLabel}{Environment.NewLine}{e}");
            ollamaApiClient?.Dispose();
            throw;
        }
    }


    public static async
    Task<ValueOrFailure<(OllamaApiClient, OllamaLocalServerProcess?), GetClientAfterEnsuringOllamaServerRunningFailInfo>>
    GetClientAfterEnsuringOllamaServerRunningAsync
    (
        Uri? serverUri = null,
        bool enableRunningLocalOllamaServer = false,
        string localOllamaHostCommand = OllamaRunningConstants.DefaultLocalOllamaHostCommand,
        CancellationToken cancellationToken = default
    )
    {
        const string methodLabel = $"[{nameof(GetClientAfterEnsuringOllamaServerRunningAsync)}]";
        const string returnErrorLabel = "[Error]";

        OllamaApiClient? ollamaApiClient = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            Debug.WriteLine($"{methodLabel} Run {nameof(GetOllamaServerVersionAsync)}. {nameof(serverUri)} = {serverUri}");
            GetOllamaServerVersionResult getOllamaServerVersionResult = await GetOllamaServerVersionAsync(serverUri, cancellationToken).ConfigureAwait(false);
            if (getOllamaServerVersionResult.IsFailure)
            {
                Debug.WriteLine($"{methodLabel} {nameof(GetOllamaServerVersionAsync)} Failed.");
                Debug.WriteLine($"{methodLabel} {nameof(GetOllamaServerVersionResult)} = {getOllamaServerVersionResult}");
            }

            ollamaApiClient = new(ensuredServerUri);

            DebugWriteLine("", $"OllamaApiClient created. Uri = {ollamaApiClient.Uri}");

            //--- Request version to given ollama server URI ---
            const string flowLabel1 = "[Request version to given ollama server URI]";

            using CancellationTokenSource timerCts = new();
            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timerCts.Token);
            timerCts.CancelAfter(TimeSpan.FromSeconds(10));

            bool versionRequestSuccessed = false;
            string? versionRequestFailMessage = null;
            try
            {
                DebugWriteLine(flowLabel1, "OllamaApiClient send GET-Version request.");
                string versionResponse = await ollamaApiClient.GetVersionAsync(combinedCts.Token).ConfigureAwait(false);

                versionRequestSuccessed = CheckSourceHasCorrectGetVersionResponse(versionResponse);
                if (!versionRequestSuccessed)
                {
                    versionRequestFailMessage = $"Invalid response. {nameof(versionResponse)} = {versionResponse}";
                    DebugWriteLine(flowLabel1, versionRequestFailMessage);
                }
            }
            catch (OperationCanceledException) when (timerCts.IsCancellationRequested)
            {
                versionRequestFailMessage = "Timeout";
                DebugWriteLine(flowLabel1, versionRequestFailMessage);
                versionRequestSuccessed = false;
            }
            //---|

            //--- Return error, if version request failed and running local ollama server disabled ---
            if (!versionRequestSuccessed && !enableRunningLocalOllamaServer)
            {
                ollamaApiClient.Dispose();
                string result =
                $"""
                Version request failed and running local ollama server disabled.
                [Version request fail message] {versionRequestFailMessage}
                """;
                DebugWriteLine(returnErrorLabel, result);
                return FailureTheory.Create
                (
                    GetClientAfterEnsuringOllamaServerRunningFailInfo.VersionRequestFailed
                    (
                        VersionRequestFailInfo.RunningLocalOllamaServerDisabled
                    ),
                    result
                );
            }
            //---|

            //--- Get local Ollama server running state ---
            var localOllamaServerRunningStateOrFailure = await GetLocalOllamaServerRunningStateAsync(localOllamaHostCommand, cancellationToken).ConfigureAwait(false);
            if (localOllamaServerRunningStateOrFailure.AsValueOrDefault == LocalOllamaServerRunningState.Running)
            {
                //--- Return error, if local Ollama server already running. ---
                ollamaApiClient.Dispose();
                string result = "Version request failed, but local Ollama server is already running.";
                DebugWriteLine(returnErrorLabel, result);
                return FailureTheory.Create
                (
                    GetClientAfterEnsuringOllamaServerRunningFailInfo.VersionRequestFailed
                    (
                        VersionRequestFailInfo.LocalOllamaServerIsAlreadyRunning
                    ),
                    result
                );
                //---|
            }
            else if (localOllamaServerRunningStateOrFailure.AsValueOrDefault != LocalOllamaServerRunningState.Idle)
            {
                //--- Return error, if local Ollama server is not running, but not idle. ---

                // Check internal error detected.
                ollamaApiClient.Dispose();
                string result =
                $"""
                Version request failed, and local Ollama server is not running, but not idle.
                {nameof(localOllamaServerRunningStateOrFailure)} = {localOllamaServerRunningStateOrFailure}
                """;
                DebugWriteLine(returnErrorLabel, result);

                return FailureTheory.Create
                (
                    GetClientAfterEnsuringOllamaServerRunningFailInfo.VersionRequestFailed
                    (
                        VersionRequestFailInfo.GetLocalOllamaServerRunningStateFailed
                        (
                            localOllamaServerRunningStateOrFailure.AsFailure.FailInfo
                        )
                    ),
                    result
                );
                //---|
            }
            //---|

            Debug.Assert(localOllamaServerRunningStateOrFailure.AsValueOrDefault == LocalOllamaServerRunningState.Idle);

            //--- Run local ollama server ---
            //---|

            //--- Return Ollama api client ---
            return (ollamaApiClient, default);
            //---|

        }
        catch (OperationCanceledException e)
        {
            ollamaApiClient?.Dispose();
            Debug.WriteLine($"{methodLabel} {e.Message}");
            return FailureTheory.Create(GetClientAfterEnsuringOllamaServerRunningFailInfo.Canceled, e.Message);
        }

        [Conditional("DEBUG")]
        static void DebugWriteLine(string subLabel, string message) =>
            Debug.WriteLine($"{methodLabel}{subLabel} {message}");
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

#if false
    public static async Task<DisposableValueOrErrorMessage<Process>>
    RunLocalOllamaServerAsync
    (
        string localOllamaHostCommand = OllamaRunningConstants.DefaultLocalOllamaHostCommand,
        CancellationToken cancellationToken = default
    )
    {
        Process? process = null;
        ManualResetEvent? observingLocalOllamaServerCompletedSignal = null;
        OllamaLocalServerStartInfo? ollamaLocalServerStartInfo = null;
        try
        {
            //--- Start new local ollama server process ---
            const string subLabel1 = "Start new local ollama server process";

            process = new();
            process.StartInfo = new()
            {
                FileName = localOllamaHostCommand,
                Arguments = "serve",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            observingLocalOllamaServerCompletedSignal = new ManualResetEvent(initialState: false);

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

            if (!process.Start())
            {
                string msg = $"Cannot start new local ollama server process. {nameof(localOllamaHostCommand)} = {localOllamaHostCommand}";
                DebugWriteLine(subLabel1, msg);
                return msg;
            }
            //---|

            //--- Observe local Ollama server successfully started ---
            const string subLabel2 = "Observe local Ollama server successfully started";

            int signaledWaitHandleIndex = await Task.Run(() =>
            {
                return WaitHandle.WaitAny([cancellationToken.WaitHandle, observingLocalOllamaServerCompletedSignal]);
            }, cancellationToken);

            //--- unsubscribe events ---
            process.ErrorDataReceived -= ErrorDataReceived_Handle;
            process.OutputDataReceived -= OutputDataReceived_Handle;
            //---|


        }
        catch (Exception e)
        {
            process?.Dispose();
            Debug.WriteLine
            (
                $"""
                [{nameof(RunLocalOllamaServerAsync)}] Unexpected exception raised. 
                exception = {e}
                """
            );
            return e.Message;
        }

        [Conditional("DEBUG")]
        static void DebugWriteLine(string subLabel, string message) =>
            Debug.WriteLine($"[{nameof(RunLocalOllamaServerAsync)}]{subLabel} {message}");
    }
#endif

    [GeneratedRegex(
        """
        time=(?<Time>\S*)\s+level=(?<Level>\S*)\s+source=(?<Source>\S*)\s+msg="Listening on (?<ListenerAddress>\S*)\s+\(version (?<Version>\S*?)\)"
        """,
        RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    internal static partial Regex GetOllamaServerLogListeningOnRegex();
}

public record OllamaLocalServerStartInfo(string ListenerAddress, string Version);

public class OllamaLocalServerProcess : IDisposable
{
    private readonly Process _process;
    private readonly StringBuilder _errorStringBuilder;
    private readonly StringBuilder _outputStringBuilder;

    internal OllamaLocalServerProcess
    (
        Process process,
        StringBuilder errorStringBuilder,
        StringBuilder outputStringBuilder
    )
    {
        Debug.Assert(process is not null);
        Debug.Assert(errorStringBuilder is not null);
        Debug.Assert(outputStringBuilder is not null);

        _process = process;
        _errorStringBuilder = errorStringBuilder;
        _outputStringBuilder = outputStringBuilder;
    }

    public void Dispose()
    {
        _process.Dispose();
        GC.SuppressFinalize(this);
    }
}