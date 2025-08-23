using Nemonuri.Failures;
using SumSharp;

namespace Nemonuri.OllamaRunning;

public static partial class ProcessStartingTheory
{
    public partial class StartOrFailResult : IValueOrFailure<bool, StartOrFailResult.FailInfo>
    {
        [UnionCase(nameof(FailCode.FileNameMissing))]
        [UnionCase(nameof(FailCode.StandardInputEncodingNotAllowed))]
        [UnionCase(nameof(FailCode.StandardOutputEncodingNotAllowed))]
        [UnionCase(nameof(FailCode.StandardErrorEncodingNotAllowed))]
        [UnionCase(nameof(FailCode.ArgumentAndArgumentListInitialized))]
        [UnionCase(nameof(FailCode.ArgumentListMayNotContainNull))]
        [UnionCase(nameof(FailCode.Disposed))]
        [UnionCase(nameof(FailCode.FileOpeningError), typeof(Win32ErrorCode))]
        [UnionCase(nameof(FailCode.PlatformNotSupported))]
        public partial struct FailInfo
        {
            public readonly FailCode FailCode => (FailCode)(Index + 1);
        }

        public enum FailCode
        {
            Unknown = 0,
            FileNameMissing = 1,
            StandardInputEncodingNotAllowed = 2,
            StandardOutputEncodingNotAllowed = 3,
            StandardErrorEncodingNotAllowed = 4,
            ArgumentAndArgumentListInitialized = 5,
            ArgumentListMayNotContainNull = 6,
            Disposed = 7,
            FileOpeningError = 8,
            PlatformNotSupported = 9
        }

        private readonly ValueOrFailure<bool, FailInfo> _internalSource;

        public StartOrFailResult(ValueOrFailure<bool, FailInfo> internalSource)
        {
            _internalSource = internalSource;
            System.Diagnostics.Debug.WriteLine("ProcessStartingTheory.StartOrFailResult constructed. " + ToString());
        }

        public static StartOrFailResult CreateAsValue(bool value) =>
            new(value);

        public static StartOrFailResult CreateAsFailure(FailInfo failInfo, string message = "") =>
            new(FailureTheory.Create(failInfo, message));

        public static StartOrFailResult CreateAsFileOpeningError(Win32ErrorCode errorCode, string message = "") =>
            new(FailureTheory.Create(FailInfo.FileOpeningError(errorCode), message));

        public bool IsValue => _internalSource.IsValue;

        public bool IsFailure => _internalSource.IsFailure;

        public bool GetValue() => _internalSource.GetValue();

        public Failure<FailInfo> GetFailure() => _internalSource.GetFailure();

        public override string ToString() =>
            "StartOrFailResult {" +
            (
                IsValue ? 
                    ("IsValue = true, Value = " + GetValue()) :
                    ("IsFailure = true, Value = " + GetFailure())
            ) + " }";
    }
}
