using System.CommandLine;
using System.Runtime.CompilerServices;
using Nemonuri.CommandLines;

namespace Nemonuri.Study.CSharpSyntaxRewriter2;

internal static class CommandParsingTheory
{
    private readonly static StrongBox<CommandParsingResultData?> _itemBox = new();

    private readonly static Argument<FileInfo> _targetFile = new Argument<FileInfo>("TargetFile")
    {
        Description = "변형할 C# 코드 파일 경로"
    }.AcceptExistingOnly();

    private readonly static RootCommand _rootCommand = new RootCommand("CSharpSyntaxRewriter Study 1")
    {
        TreatUnmatchedTokensAsErrors = true
    }.With(_targetFile)
    .WithFactoryAction
    (
        static (ParseResult pr, out CommandParsingResultData? item) =>
        {
            item = new()
            {
                TargetFile = pr.GetRequiredValue(_targetFile)
            };
            return 0;
        },
        _itemBox
    );

    private readonly static CommandLineConfiguration _parser = new(_rootCommand)
    {
        EnablePosixBundling = false
    };

    public static CommandParsingResultData? Parse(string[] args)
    {
        _parser.Parse(args).Invoke();
        return _itemBox.Value;
    }
}

internal class CommandParsingResultData()
{
    public required FileInfo TargetFile { get; init; }
}