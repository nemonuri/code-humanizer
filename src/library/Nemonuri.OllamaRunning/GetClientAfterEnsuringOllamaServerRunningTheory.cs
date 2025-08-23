using SumSharp;

namespace Nemonuri.OllamaRunning;

#if false
public static partial class OllamaRunningTheory
{
    public partial class GetClientAfterEnsuringOllamaServerRunningResult
    {
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
    }

}
#endif