using Nemonuri.Failures;
using SumSharp;

namespace Nemonuri.OllamaRunning;

public static partial class OllamaRunningTheory
{
    public partial class GetLocalOllamaServerRunningStateResult :
        IValueOrFailure<LocalOllamaServerRunningState, GetLocalOllamaServerRunningStateResult.FailInfo>
    {
        [UnionCase(nameof(FailCode.Canceled))]
        [UnionCase(nameof(FailCode.ProcessStartFailed), typeof(ProcessStartingTheory.StartOrFailResult.FailInfo))]
        [UnionCase(nameof(FailCode.SolveLocalOllamaServerRunningStateFailed))]
        public partial struct FailInfo
        {
            public readonly FailCode FailCode =>
                (FailCode)(Index + 1);
        }

        public enum FailCode
        {
            Unknown = 0,
            Canceled = 1,
            ProcessStartFailed = 2,
            SolveLocalOllamaServerRunningStateFailed = 3
        }

        private readonly ValueOrFailure<LocalOllamaServerRunningState, FailInfo> _internalSource;

        public GetLocalOllamaServerRunningStateResult(ValueOrFailure<LocalOllamaServerRunningState, FailInfo> internalSource)
        {
            _internalSource = internalSource;
        }

        public static GetLocalOllamaServerRunningStateResult CreateAsValue(LocalOllamaServerRunningState value) =>
            new(value);

        public static GetLocalOllamaServerRunningStateResult CreateAsFailure(FailInfo failInfo, string message = "") =>
            new(FailureTheory.Create(failInfo, message));

        public static GetLocalOllamaServerRunningStateResult CreateAsProcessStartFailed(ProcessStartingTheory.StartOrFailResult.FailInfo value, string message = "") =>
            new(FailureTheory.Create(FailInfo.ProcessStartFailed(value), message));

        public bool IsValue => _internalSource.IsValue;

        public bool IsFailure => _internalSource.IsFailure;

        public LocalOllamaServerRunningState GetValue() => _internalSource.GetValue();

        public Failure<FailInfo> GetFailure() => _internalSource.GetFailure();
    }
}
