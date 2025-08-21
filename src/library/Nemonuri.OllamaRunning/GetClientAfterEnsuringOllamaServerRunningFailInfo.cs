using SumSharp;

namespace Nemonuri.OllamaRunning;

[UnionCase(nameof(GetClientAfterEnsuringOllamaServerRunningFailCode.Canceled))]
[UnionCase(
    nameof(GetClientAfterEnsuringOllamaServerRunningFailCode.VersionRequestFailed),
    typeof(VersionRequestFailInfo)
)]
public partial struct GetClientAfterEnsuringOllamaServerRunningFailInfo
{
    public readonly GetClientAfterEnsuringOllamaServerRunningFailCode FailCode =>
        (GetClientAfterEnsuringOllamaServerRunningFailCode)(Index + 1);
}

public enum GetClientAfterEnsuringOllamaServerRunningFailCode
{
    Unknown = 0,
    Canceled = 1,
    VersionRequestFailed = 2
}


[UnionCase(nameof(VersionRequestFailedExtraFailCode.RunningLocalOllamaServerDisabled))]
[UnionCase(nameof(VersionRequestFailedExtraFailCode.LocalOllamaServerIsAlreadyRunning))]
[UnionCase(
    nameof(VersionRequestFailedExtraFailCode.GetLocalOllamaServerRunningStateFailed),
    typeof(GetLocalOllamaServerRunningStateFailInfo)
)]
public partial struct VersionRequestFailInfo
{
    public readonly VersionRequestFailedExtraFailCode FailCode =>
        (VersionRequestFailedExtraFailCode)(Index + 1);
}

public enum VersionRequestFailedExtraFailCode
{
    Unknown = 0,
    RunningLocalOllamaServerDisabled = 1,
    LocalOllamaServerIsAlreadyRunning = 2,
    GetLocalOllamaServerRunningStateFailed = 3
}