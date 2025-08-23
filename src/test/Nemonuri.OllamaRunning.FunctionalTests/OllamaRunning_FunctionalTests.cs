namespace Nemonuri.OllamaRunning.FunctionalTests;

using Nemonuri.Failures;

public class OllamaRunningTheory_Test
{
    // Original: https://github.com/dotnet/runtime/blob/de0650bd3b58c330ef685a549205c5aeb5975291/src/libraries/Common/tests/System/Net/Http/HttpClientHandlerTestBase.cs#L19C52-L19C85
    private static readonly Uri InvalidUri = new("http://nosuchhost.invalid");

    private readonly ITestOutputHelper _testOutput;

    public OllamaRunningTheory_Test(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

    [Fact]
    public async Task GetClientAfterEnsuringOllamaServerRunningAsync_WhenUriIsInvalidAndDisableRunningLocalOllamaServer_ShouldBeErrorMessage()
    {
        // Arrange

        // Act
        OllamaRunningTheory.GetClientAfterEnsuringOllamaServerRunningResult actual = await OllamaRunningTheory.GetClientAfterEnsuringOllamaServerRunningAsync
        (
            serverUri: InvalidUri,
            enableRunningLocalOllamaServer: false,
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        if (actual.IsValue)
        {
            actual.GetValue().Item1.Dispose();
        }
        Assert.True(actual.IsFailure);
        _testOutput.WriteLine(actual.GetMessage());
    }
}

