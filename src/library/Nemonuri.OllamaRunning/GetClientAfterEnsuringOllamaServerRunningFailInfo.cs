using SumSharp;

namespace Nemonuri.OllamaRunning;

#if false
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
#endif