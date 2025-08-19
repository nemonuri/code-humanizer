using SumSharp;
using OllamaSharp;

namespace Nemonuri.OllamaRunning;

[UnionCase(nameof(OllamaSharp.OllamaApiClient), typeof(OllamaApiClient))]
[UnionCase("ErrorMessage", typeof(string))]
public partial class OllamaApiClientOrErrorMessage : IDisposable
{
    public void Dispose()
    {
        if (this.AsOllamaApiClientOrDefault is { } ollamaClient)
        {
            ollamaClient.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
