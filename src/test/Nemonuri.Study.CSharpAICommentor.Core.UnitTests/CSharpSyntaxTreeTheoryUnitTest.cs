using Microsoft.Extensions.Logging;

namespace Nemonuri.Study.CSharpAICommentor.Core.UnitTests;

public class CSharpSyntaxTreeTheoryUnitTest
{
    private readonly ITestOutputHelper _output;
    private readonly ILoggerFactory _loggerFactory;

    public CSharpSyntaxTreeTheoryUnitTest(ITestOutputHelper output)
    {
        _output = output;
        _loggerFactory = LoggerFactory.Create
        (
            builder => builder
                .AddXUnit(_output)
                .SetMinimumLevel(LogLevel.Debug)
        );
    }

    [Theory]
    [MemberData(nameof(Data1))]
    public async Task CreateCSharpSyntaxTreeFromFile
    (
        string fileName,
        bool isValueExpected
    )
    {
        // Arrange
        ILogger logger = _loggerFactory.CreateLogger(nameof(CSharpSyntaxTreeTheory));
        string filePath = Path.Combine(AppContext.BaseDirectory, $"res/{fileName}.txt");
        FileInfo fileInfo = new(filePath);

        // Act
        var actual = await CSharpSyntaxTreeTheory.CreateCSharpSyntaxTreeFromFileAsync
        (
            fileInfo, logger, TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(isValueExpected, actual.IsValue);
    }

    public static TheoryData<string, bool> Data1 => new()
    {
        { "valid_syntax", true },
        { "invalid_syntax", true },
        { "not_exist", false }
    };
}
