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
    public async Task GetLocalOllamaServerRunningStateAsync_WhenLocalOllamaServerIsNotRunning_ShouldIdleValue()
    {
        // Arrange
        Console.WriteLine("Please do these:");
        Console.WriteLine($"1. Check '{_entryFixture.ValidLocalOllamaHostCommand}' is correct local Ollama host command.");
        Console.WriteLine($"2. Check local Ollama server is not running.");
        Console.WriteLine("Type 'y' and press 'Enter' to continue..");
        Assert.Equal("y", Console.ReadLine());

        // Act
        OllamaRunningTheory.GetLocalOllamaServerRunningStateResult actual = await OllamaRunningTheory.GetLocalOllamaServerRunningStateAsync
        (
            localOllamaHostCommand: _entryFixture.ValidLocalOllamaHostCommand,
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(actual.IsValue);
        Assert.Equal(LocalOllamaServerRunningState.Idle, actual.GetValue());
    }

    [Fact(Skip = ManualTestDisabled, SkipUnless = nameof(EnableManualTest))]
    public async Task GetLocalOllamaServerRunningStateAsync_WhenLocalOllamaServerIsRunning_ShouldRunningValue()
    {
        // Arrange
        Console.WriteLine("Please do these:");
        Console.WriteLine($"1. Check '{_entryFixture.ValidLocalOllamaHostCommand}' is correct local Ollama host command.");
        Console.WriteLine($"2. Check local Ollama server is running.");
        Console.WriteLine("Type 'y' and press 'Enter' to continue..");
        Assert.Equal("y", Console.ReadLine());

        // Act
        OllamaRunningTheory.GetLocalOllamaServerRunningStateResult actual = await OllamaRunningTheory.GetLocalOllamaServerRunningStateAsync
        (
            localOllamaHostCommand: _entryFixture.ValidLocalOllamaHostCommand,
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(actual.IsValue);
        Assert.Equal(LocalOllamaServerRunningState.Running, actual.GetValue());
    }

    [Fact(Skip = ManualTestDisabled, SkipUnless = nameof(EnableManualTest))]
    public async Task GetLocalOllamaServerRunningStateAsync_WhenLocalOllamaHostCommandIsWrong_ShouldErrorMessage()
    {
        // Arrange
        Console.WriteLine("Please do these:");
        Console.WriteLine($"1. Check '{_entryFixture.InvalidLocalOllamaHostCommand}' is wrong local Ollama host command.");
        Console.WriteLine("Type 'y' and press 'Enter' to continue..");
        Assert.Equal("y", Console.ReadLine());

        // Act
        OllamaRunningTheory.GetLocalOllamaServerRunningStateResult actual = await OllamaRunningTheory.GetLocalOllamaServerRunningStateAsync
        (
            localOllamaHostCommand: _entryFixture.InvalidLocalOllamaHostCommand,
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(actual.IsFailure);
    }

    [Theory(Skip = ManualTestDisabled, SkipUnless = nameof(EnableManualTest))]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetClientAfterEnsuringOllamaServerRunningAsync_WhenLocalOllamaServerIsRunning_ShouldValueAndServerIsNull
    (
        bool isLocalOllamaHostCommandCorrect
    )
    {
        // Arrange
        Console.WriteLine("Please do these:");
        if (isLocalOllamaHostCommandCorrect)
        {
            Console.WriteLine($"1. Check '{_entryFixture.ValidLocalOllamaHostCommand}' is correct local Ollama host command.");
        }
        else
        {
            Console.WriteLine($"1. Check '{_entryFixture.InvalidLocalOllamaHostCommand}' is wrong local Ollama host command.");
        }
        Console.WriteLine("2. Check local Ollama server is running.");
        Console.WriteLine($"3. Check local Ollama server URI is {_entryFixture.OllamaServerUri}");
        Console.WriteLine("Type 'y' and press 'Enter' to continue..");
        Assert.Equal("y", Console.ReadLine());

        // Act
        OllamaRunningTheory.GetClientAfterEnsuringOllamaServerRunningResult actual = await OllamaRunningTheory.GetClientAfterEnsuringOllamaServerRunningAsync
        (
            serverUri: new Uri(_entryFixture.OllamaServerUri),
            localOllamaHostCommand: (isLocalOllamaHostCommandCorrect ? _entryFixture.ValidLocalOllamaHostCommand : _entryFixture.InvalidLocalOllamaHostCommand),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(actual.IsValue);
        Assert.Null(actual.GetValue().Server);
    }
}
