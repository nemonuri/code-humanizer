namespace Nemonuri.OllamaRunning.ManualTests;

public class OllamaRunningTheoryTest : IClassFixture<OllamaRunningTheoryTestEntryFixture>
{
    public static bool EnableManualTest => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MANUAL_TEST"));
    private const string ManualTestDisabled =
    """
    Manual Test is Disabled.
    Set environment variable "MANUAL_TEST" to non-empty string.
    """;

    private readonly ITestOutputHelper _output;
    private readonly IReadOnlyOllamaRunningTheoryTestEntryFixture _entryFixture;

    public OllamaRunningTheoryTest
    (
        ITestOutputHelper testOutput,
        OllamaRunningTheoryTestEntryFixture entryFixture
    )
    {
        _output = testOutput;
        _entryFixture = entryFixture;
    }

    [Fact]
    public void Dummy()
    {
        _output.WriteLine($"{nameof(_entryFixture)} = {_entryFixture}");
    }

    [Fact(Skip = ManualTestDisabled, SkipUnless = nameof(EnableManualTest))]
    public async Task CheckLocalOllamaServerRunningStateAsync_WhenLocalOllamaServerIsNotRunning_ShouldIdleValue()
    {
        // Arrange
        Console.WriteLine("Please do these:");
        Console.WriteLine($"1. Check '{_entryFixture.ValidLocalOllamaHostCommand}' is correct local Ollama host command.");
        Console.WriteLine($"2. Check local Ollama server is not running.");
        Console.WriteLine("Type 'y' and press 'Enter' to continue..");
        Assert.Equal("y", Console.ReadLine());

        // Act
        ValueOrErrorMessage<LocalOllamaServerRunningState> actual = await OllamaRunningTheory.CheckLocalOllamaServerRunningStateAsync
        (
            localOllamaHostCommand: _entryFixture.ValidLocalOllamaHostCommand,
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        _output.WriteLine(actual.ToString());
        Assert.True(actual.IsValue);
        Assert.Equal(LocalOllamaServerRunningState.Idle, actual.AsValueOrDefault);
    }
}
