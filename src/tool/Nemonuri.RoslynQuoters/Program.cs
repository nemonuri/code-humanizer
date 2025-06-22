namespace Nemonuri.RoslynQuoters;

/// <summary>
/// 프로그램
/// </summary>
public static class Program
{
    /// <summary>
    /// 이 프로그램의 진입점입니다.
    /// </summary>
    /// <param name="args">명령어</param>
    public static void Main(string[] args)
    {
        var parseResult = CommandParsingTheory.Parse(args);

        Console.WriteLine(parseResult.TargetFile);
    }
}