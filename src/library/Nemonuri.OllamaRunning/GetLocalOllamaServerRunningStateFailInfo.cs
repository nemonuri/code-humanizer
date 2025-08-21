
using SumSharp;

namespace Nemonuri.OllamaRunning;

[UnionCase(nameof(GetLocalOllamaServerRunningStateFailCode.Canceled))]
[UnionCase(nameof(GetLocalOllamaServerRunningStateFailCode.ProcessStartFailed), typeof(ProcessStartFailInfo))]
[UnionCase(nameof(GetLocalOllamaServerRunningStateFailCode.SolveLocalOllamaServerRunningStateFailed))]
public partial struct GetLocalOllamaServerRunningStateFailInfo
{
    public readonly GetLocalOllamaServerRunningStateFailCode FailCode =>
        (GetLocalOllamaServerRunningStateFailCode)(Index + 1);
}

public enum GetLocalOllamaServerRunningStateFailCode
{
    Unknown = 0,
    Canceled = 1,
    ProcessStartFailed = 2,
    SolveLocalOllamaServerRunningStateFailed = 3
}