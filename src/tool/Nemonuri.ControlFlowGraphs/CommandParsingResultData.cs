namespace Nemonuri.ControlFlowGraphs;

/// <summary>
/// 이 앱이 명령어를 구문 분석한 결과 데이터를 나타내는 클래스입니다.
/// </summary>
public class CommandParsingResultData()
{
    /// <summary>
    /// 대상 C# 코드를 담고 있는 파일
    /// </summary>
    public required FileInfo TargetFile { get; init; }
}
