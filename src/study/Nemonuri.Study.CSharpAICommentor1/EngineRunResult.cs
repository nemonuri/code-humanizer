
namespace Nemonuri.Study.CSharpAICommentor1;

public class EngineRunResult
{
    public EngineRunResult(string? errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public string? ErrorMessage { get; }
}