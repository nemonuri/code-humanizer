using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using OllamaSharp;

namespace Nemonuri.OllamaRunning;

public static partial class OllamaRunningTheory
{
    public static async Task<OllamaApiClientOrErrorMessage>
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

        Uri ensuredServerUri = serverUri ?? OllamaRunningConstants.DefaultOllamaServerUri;

        OllamaApiClient? ollamaApiClient = null;
        try
        {
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

                versionRequestSuccessed = CheckSourceHasCorrectGetVersionResponseClue(versionResponse);
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
                return result;
            }
            //---|

            //--- Get local Ollama server running state ---
            var localOllamaServerRunningStateOrError = await GetLocalOllamaServerRunningStateAsync(localOllamaHostCommand, cancellationToken).ConfigureAwait(false);
            if (localOllamaServerRunningStateOrError.AsValueOrDefault == LocalOllamaServerRunningState.Running)
            {
                //--- Return error, if local Ollama server already running. ---
                ollamaApiClient.Dispose();
                string result = "Version request failed, but local Ollama server is already running.";
                DebugWriteLine(returnErrorLabel, result);
                return result;
                //---|
            }
            else if (localOllamaServerRunningStateOrError.AsValueOrDefault != LocalOllamaServerRunningState.Idle)
            {
                //--- Return error, if local Ollama server is not running, but not idle. ---

                // Check internal error detected.
                bool internalErrorDetected = localOllamaServerRunningStateOrError.IsErrorMessage;

                ollamaApiClient.Dispose();
                string result = internalErrorDetected ?
                    $"""
                    Version request failed, and local Ollama server is not running, but not idle, and error is detected in {nameof(GetLocalOllamaServerRunningStateAsync)}.
                    {nameof(localOllamaServerRunningStateOrError.ErrorMessage)} = {localOllamaServerRunningStateOrError.AsErrorMessage}
                    """
                    :
                    $"""
                    Version request failed, and local Ollama server is not running, but not idle, and error is not detected in {nameof(GetLocalOllamaServerRunningStateAsync)}.
                    Value = {localOllamaServerRunningStateOrError.AsValue}
                    """;
                DebugWriteLine(returnErrorLabel, result);
                return result;
                //---|
            }
            //---|

            Debug.Assert(localOllamaServerRunningStateOrError.AsValueOrDefault == LocalOllamaServerRunningState.Idle);

            //--- Run local ollama server ---
            //---|

            //--- Run local ollama client, if server of given URI is not alive ---
            if (!versionRequestSuccessed)
            {
                if (!enableRunningLocalOllamaServer)
                {
                    ollamaApiClient.Dispose();
                    return OllamaApiClientOrErrorMessage.ErrorMessage(versionRequestFailMessage ?? "");
                }
            }
            //---|

            //--- Return Ollama api client ---
            return OllamaApiClientOrErrorMessage.OllamaApiClient(ollamaApiClient);
            //---|

        }
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
            ollamaApiClient?.Dispose();
            string result = e.Message;
            Debug.WriteLine($"{methodLabel}{returnErrorLabel} {result}");
            return result;
        }

        [Conditional("DEBUG")]
        static void DebugWriteLine(string subLabel, string message) =>
            Debug.WriteLine($"{methodLabel}{subLabel} {message}");
    }

    public static async Task<ValueOrErrorMessage<LocalOllamaServerRunningState>>
    GetLocalOllamaServerRunningStateAsync
    (
        string localOllamaHostCommand = OllamaRunningConstants.DefaultLocalOllamaHostCommand,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
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
                [{nameof(GetLocalOllamaServerRunningStateAsync)}] Starting local ollama host process. 
                {nameof(process)} = {process}
                """
            );

            if (!process.Start())
            {
                return "Cannot start local Ollama server version checking process.";
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
            if (CheckSourceHasCorrectGetVersionResponseClue(stdoutString))
            {
                Debug.WriteLine
                (
                    $"""
                    [{nameof(GetLocalOllamaServerRunningStateAsync)}] local ollama server running state solved. 
                    result = {LocalOllamaServerRunningState.Running}
                    """
                );
                return LocalOllamaServerRunningState.Running;
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
                    [{nameof(GetLocalOllamaServerRunningStateAsync)}] local ollama server running state solved. 
                    result = {LocalOllamaServerRunningState.Idle}
                    """
                );
                return LocalOllamaServerRunningState.Idle;
            }
            else
            {
                Debug.WriteLine
                (
                    $"""
                    [{nameof(GetLocalOllamaServerRunningStateAsync)}] Cannot solve local ollama server running state. 
                    result = {errorString}
                    """
                );
                return errorString;
            }
            //---|
        }
        catch (Exception e)
        {
            Debug.WriteLine
            (
                $"""
                [{nameof(GetLocalOllamaServerRunningStateAsync)}] Unexpected exception raised. 
                exception = {e}
                """
            );
            return $"{e.Message}";
        }
    }

    private static bool CheckSourceHasCorrectGetVersionResponseClue(string source) =>
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