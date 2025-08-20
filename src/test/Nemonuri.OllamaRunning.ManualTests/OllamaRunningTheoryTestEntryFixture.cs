namespace Nemonuri.OllamaRunning.ManualTests;

public record OllamaRunningTheoryTestEntryFixture : IReadOnlyOllamaRunningTheoryTestEntryFixture
{
    public string ValidLocalOllamaHostCommand { get; set; } = "ollama";
    public string InvalidLocalOllamaHostCommand { get; set; } = "ollama2";

    public OllamaRunningTheoryTestEntryFixture()
    {
        Console.WriteLine($"[{nameof(OllamaRunningTheoryTestEntryFixture)}] Created");
        Console.WriteLine($"[{nameof(OllamaRunningTheoryTestEntryFixture)}] {Environment.NewLine}{this}");
    }
}

public interface IReadOnlyOllamaRunningTheoryTestEntryFixture
{
    string ValidLocalOllamaHostCommand { get; }
    string InvalidLocalOllamaHostCommand { get; }
}