using System.CommandLine;
using System.Runtime.CompilerServices;

namespace Nemonuri.RoslynQuoters;

/// <summary>
/// 이 앱이 명령어를 구문 분석하는 방법에 대한 이롬입니다.
/// </summary>
public static class CommandParsingTheory
{
    private readonly static StrongBox<CommandParsingResultData?> _itemBox = new();

    private readonly static Argument<FileInfo> _targetFile = new Argument<FileInfo>("TargetFile")
    { 
        Description = "분석할 C# 코드 파일 경로"
    }.AcceptExistingOnly();

    private readonly static RootCommand _rootCommand = new RootCommand("Roslyn Quoter")
    {
        TreatUnmatchedTokensAsErrors = true
    }.With(_targetFile)
    .WithFactoryAction
    (
        static (ParseResult pr, out CommandParsingResultData? item) =>
        {
            item = new CommandParsingResultData()
            {
                TargetFile = pr.GetRequiredValue(_targetFile)
            };
            return 0;
        },
        _itemBox
    );

    private readonly static CommandLineConfiguration _parser = new CommandLineConfiguration(_rootCommand)
    {
        EnablePosixBundling = false
    };

    /// <summary>
    /// 주어진 명령어를 구문 분석해 그 결과를 반환합니다.
    /// </summary>
    /// <param name="args">주어진 명령어</param>
    /// <returns>구문 분석 결과</returns>
    public static CommandParsingResultData? Parse(string[] args)
    {
        _parser.Parse(args).Invoke();
        return _itemBox.Value;
    }
}
