using System.Diagnostics;
using System.Text;
using CommunityToolkit.Diagnostics;
using OllamaSharp;

namespace Nemonuri.OllamaRunning;

public static class OllamaRunningTheory
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
        Uri ensuredServerUri = serverUri ?? OllamaRunningConstants.DefaultOllamaServerUri;

        OllamaApiClient? ollamaApiClient = null;
        try
        {
            ollamaApiClient = new(ensuredServerUri);

            //--- Ping to ollama server, to test server of given URI is alive ---
            using CancellationTokenSource timerCts = new();
            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timerCts.Token);
            timerCts.CancelAfter(TimeSpan.FromSeconds(10));

            bool pingSuccessed = false;
            string? pingFailMessage = null;
            try
            {
                string version = await ollamaApiClient.GetVersionAsync(combinedCts.Token).ConfigureAwait(false);
                pingSuccessed = !string.IsNullOrWhiteSpace(version);
                if (!pingSuccessed)
                {
                    pingFailMessage = $"{GetDebugLogPrefix()}[Ping to ollama server] Invalid {nameof(ollamaApiClient.GetVersionAsync)} result. Value = {version}";
                }
            }
            catch (OperationCanceledException) when (timerCts.IsCancellationRequested)
            {
                pingFailMessage = $"{GetDebugLogPrefix()}[Ping to ollama server] Timeout";
                pingSuccessed = false;
            }
            //---|

            //--- Run local ollama client, if server of given URI is not alive ---
            if (!pingSuccessed)
            {
                if (!enableRunningLocalOllamaServer)
                {
                    ollamaApiClient.Dispose();
                    return OllamaApiClientOrErrorMessage.ErrorMessage(pingFailMessage ?? "");
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
            return OllamaApiClientOrErrorMessage.ErrorMessage
            (
                $"{GetDebugLogPrefix()}[Error] {e.Message}"
            );
        }

        static string GetDebugLogPrefix() => $"[{nameof(OllamaRunningTheory)}][{nameof(GetClientAfterEnsuringOllamaServerRunningAsync)}]";
    }

    public static async Task<ValueOrErrorMessage<LocalOllamaServerRunningState>>
    CheckLocalOllamaServerRunningStateAsync
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
                [{nameof(CheckLocalOllamaServerRunningStateAsync)}] Starting local ollama host process. 
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
                [{nameof(CheckLocalOllamaServerRunningStateAsync)}] Process closed. 
                {nameof(errorString)} = {errorString}
                {nameof(stdoutString)} = {stdoutString}
                """
            );
            //---|

            //--- Solve local ollama server running state ---
            if (stdoutString.Contains("ollama version is"))
            {
                Debug.WriteLine
                (
                    $"""
                    [{nameof(CheckLocalOllamaServerRunningStateAsync)}] local ollama server running state solved. 
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
                    [{nameof(CheckLocalOllamaServerRunningStateAsync)}] local ollama server running state solved. 
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
                    [{nameof(CheckLocalOllamaServerRunningStateAsync)}] Cannot solve local ollama server running state. 
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
                [{nameof(CheckLocalOllamaServerRunningStateAsync)}] Unexpected exception raised. 
                exception = {e}
                """
            );
            return $"{e.Message}";
        }
    }   
}