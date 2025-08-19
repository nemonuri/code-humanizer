using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using OllamaSharp;

namespace Nemonuri.OllamaRunning;

public static class OllamaRunningTheory
{
    public static async Task<OllamaApiClientOrErrorMessage>
    GetClientAfterEnsuringOllamaServerRunningAsync
    (
        HttpClient httpClient,
        Uri? serverUri = null,
        string ollamaAppCommand = OllamaRunningConstants.DefaultOllamaAppCommand,
        CancellationToken cancellationToken = default
    )
    {
        Guard.IsNotNull(httpClient);

        Uri? oldHttpClientBaseAddress = httpClient.BaseAddress;
        Uri ensuredServerUri = serverUri ?? OllamaRunningConstants.DefaultOllamaServerUri;

        try
        {
            httpClient.BaseAddress = ensuredServerUri;
            OllamaApiClient ollamaApiClient = new(httpClient);

            //--- Ping to ollama server, to test server of given URI is alive ---
            using CancellationTokenSource timerCts = new();
            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timerCts.Token);
            timerCts.CancelAfter(TimeSpan.FromSeconds(10));

            bool pingSuccessed = false;
            try
            {
                string version = await ollamaApiClient.GetVersionAsync(combinedCts.Token).ConfigureAwait(false);
                pingSuccessed = !string.IsNullOrWhiteSpace(version);
            }
            catch (OperationCanceledException) when (timerCts.IsCancellationRequested)
            {
                Debug.WriteLine($"{GetDebugLogPrefix()}[Ping to ollama server] Timeout");
                pingSuccessed = false;
            }
            //---|

            //--- Run local ollama client, if server of given URI is not alive ---
            if (!pingSuccessed)
            {

            }
            //---|

            //--- Return Ollama api client ---
            return OllamaApiClientOrErrorMessage.OllamaApiClient(ollamaApiClient);
            //---|

        }
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
            return OllamaApiClientOrErrorMessage.ErrorMessage
            (
                $"{GetDebugLogPrefix()}[Error] {e.Message}"
            );
        }
        finally
        {
            httpClient.BaseAddress = oldHttpClientBaseAddress;
        }

        static string GetDebugLogPrefix() => $"[{nameof(OllamaRunningTheory)}][{nameof(GetClientAfterEnsuringOllamaServerRunningAsync)}]";
    }
}
