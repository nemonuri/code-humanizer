namespace Nemonuri.OllamaRunning.ManualTests;

public record OllamaRunningTheoryTestEntryFixture() : IReadOnlyOllamaRunningTheoryTestEntryFixture
{
    public string ValidLocalOllamaHostCommand { get; set; } = "ollama";
    public string InvalidLocalOllamaHostCommand { get; set; } = "ollama2";
    public string OllamaServerUri { get; set; } = "http://localhost:11434/";
}

public interface IReadOnlyOllamaRunningTheoryTestEntryFixture
{
    string ValidLocalOllamaHostCommand { get; }
    string InvalidLocalOllamaHostCommand { get; }
    string OllamaServerUri { get; }
}