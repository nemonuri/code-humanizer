namespace Nemonuri.OllamaRunning.ManualTests;


public class OllamaRunningTheoryTest : IClassFixture<OllamaRunningTheoryTestEntryFixture>
{
    public static bool EnableManualTest => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MANUAL_TEST"));
    private const string ManualTestDisabled =
    """
    Manual Test is Disabled.
    Set environment variable "MANUAL_TEST" to non-empty string.
    """;

    private readonly ITestOutputHelper _testOutput;
    private readonly IReadOnlyOllamaRunningTheoryTestEntryFixture _entryFixture;

    public OllamaRunningTheoryTest
    (
        ITestOutputHelper testOutput,
        OllamaRunningTheoryTestEntryFixture entryFixture
    )
    {
        _testOutput = testOutput;
        _entryFixture = entryFixture;
    }

    [Fact(Skip = ManualTestDisabled, SkipUnless = nameof(EnableManualTest))]
    public void CheckLocalOllamaServerRunningStateAsync()
    {
        // Arrange

        // Act
    }
}
