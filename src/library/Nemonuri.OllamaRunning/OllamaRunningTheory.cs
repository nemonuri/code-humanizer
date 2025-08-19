using System.Diagnostics;
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
        string localOllamaServerCommand = OllamaRunningConstants.DefaultOllamaAppCommand,
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
}
