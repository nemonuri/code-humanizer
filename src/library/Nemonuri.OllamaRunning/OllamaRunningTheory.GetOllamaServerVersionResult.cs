using Nemonuri.Failures;
using SumSharp;
using OllamaSharp;

namespace Nemonuri.OllamaRunning;

public static partial class OllamaRunningTheory
{
    public partial class GetOllamaServerVersionResult : IValueOrFailure<(OllamaApiClient, string), GetOllamaServerVersionResult.FailInfo>
    {
        [UnionCase(nameof(FailCode.Cancel))]
        [UnionCase(nameof(FailCode.InvalidResponse), typeof(string))]
        [UnionCase(nameof(FailCode.TimeOut), typeof(TimeSpan))]
        public partial struct FailInfo
        {
            public readonly FailCode FailCode => (FailCode)(Index + 1);
        }

        public enum FailCode
        {
            Unknown = 0,
            Cancel = 1,
            InvalidResponse = 2,
            TimeOut = 3
        }

        private readonly ValueOrFailure<(OllamaApiClient, string), FailInfo> _internalSource;

        public GetOllamaServerVersionResult(ValueOrFailure<(OllamaApiClient, string), FailInfo> internalSource)
        {
            _internalSource = internalSource;
        }

        public static GetOllamaServerVersionResult CreateAsValue((OllamaApiClient, string) value) =>
            new(value);

        public static GetOllamaServerVersionResult CreateAsFailure(FailInfo failInfo, string message = "") =>
            new(FailureTheory.Create(failInfo, message));

        public static GetOllamaServerVersionResult CreateAsInvalidResponse(string value, string message = "") =>
            new(FailureTheory.Create(FailInfo.InvalidResponse(value), message));

        public static GetOllamaServerVersionResult CreateAsTimeOut(TimeSpan value, string message = "") =>
            new(FailureTheory.Create(FailInfo.TimeOut(value), message));

        public bool IsValue => _internalSource.IsValue;

        public bool IsFailure => _internalSource.IsFailure;

        public (OllamaApiClient, string) GetValue() => _internalSource.GetValue();

        public Failure<FailInfo> GetFailure() => _internalSource.GetFailure();
    }

}
