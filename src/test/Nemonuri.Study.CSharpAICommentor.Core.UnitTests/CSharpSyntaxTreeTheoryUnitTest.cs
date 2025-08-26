using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nemonuri.Failures;
using Fc1 = Nemonuri.Study.CSharpAICommentor.CSharpSyntaxTreeTheory.CreateCompilationUnitRootedCSharpSyntaxTreeInfoResult.FailCode;

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
    public async Task CreateCSharpSyntaxTreeFromFileAsync
    (
        string fileName,
        bool expectedIsValue
    )
    {
        // Arrange
        ILogger logger = _loggerFactory.CreateLogger(nameof(CreateCSharpSyntaxTreeFromFileAsync));
        string filePath = Path.Combine(AppContext.BaseDirectory, $"res/{fileName}.txt");
        FileInfo fileInfo = new(filePath);

        // Act
        var actual = await CSharpSyntaxTreeTheory.CreateCSharpSyntaxTreeFromFileAsync
        (
            fileInfo, logger, TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expectedIsValue, actual.IsValue);
    }

    public static TheoryData<string, bool> Data1 => new()
    {
        { "valid_syntax", true },
        { "invalid_syntax", true },
        { "not_exist", false }
    };

    [Theory]
    [MemberData(nameof(Data2))]
    public async Task CreateCompilationUnitRootedCSharpSyntaxTreeInfoAsync
    (
        string fileName,
        bool expectedIsValue,
        Fc1 expectedFailCode,
        bool expectedIsMissing
    )
    {
        // Arrange
        ILogger logger = _loggerFactory.CreateLogger(nameof(CreateCompilationUnitRootedCSharpSyntaxTreeInfoAsync));
        string filePath = Path.Combine(AppContext.BaseDirectory, $"res/{fileName}.txt");
        FileInfo fileInfo = new(filePath);

        // Act
        var actual = await CSharpSyntaxTreeTheory.CreateCompilationUnitRootedCSharpSyntaxTreeInfoAsync
        (
            fileInfo, logger, TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expectedIsValue, actual.IsValue);
        if (actual.IsFailure)
        {
            Assert.Equal(expectedFailCode, actual.GetFailInfo().FailCode);
        }
        if (actual.IsValue)
        {
            Assert.Equal(expectedIsMissing, actual.GetValue().IsMissing);
        }
    }

    public static TheoryData<string, bool, Fc1, bool> Data2 => new()
    {
        { "valid_syntax", true, default, false },
        { "invalid_syntax", true, default, true },
        { "not_exist", false, Fc1.CreateCSharpSyntaxTreeFromFileFailed, default }
    };

    [Theory]
    [MemberData(nameof(Data3))]
    public async Task TrySeparateComplexArgumentExpressions
    (
        string fileName
    )
    {
        // Arrange
        string filePath = Path.Combine(AppContext.BaseDirectory, $"res/{fileName}.txt");
        FileInfo fileInfo = new(filePath);

        // Act
        var compOrFail = await CSharpSyntaxTreeTheory.CreateCompilationUnitRootedCSharpSyntaxTreeInfoAsync
        (
            fileInfo, default, TestContext.Current.CancellationToken
        );

        var comp = compOrFail.GetValue().Root;
        bool actualSuccess = CSharpSyntaxTreeTheory.TrySeparateComplexArgumentExpressions(comp, out var actualResult);

        Assert.True(actualSuccess);
        _output.WriteLine
        (
            Environment.NewLine + actualResult!.NormalizeWhitespace
            (
                indentation: "    ",
                elasticTrivia: true
            ).ToFullString()
        );
    }

    public static TheoryData<string> Data3 => new()
    {
        "valid_syntax",
        "Example1.cs"
    };
}
