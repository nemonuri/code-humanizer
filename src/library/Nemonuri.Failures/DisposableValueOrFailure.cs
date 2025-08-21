
using SumSharp;

namespace Nemonuri.Failures;

[UnionCase("Value", nameof(TValue))]
[UnionCase("Failure", "Failure<TFailureData>")]
public partial class DisposableValueOrFailure<TValue, TFailureData> : IDisposable
    where TValue : IDisposable
{
    public void Dispose()
    {
        if (AsValueOrDefault is { } value) { value.Dispose(); }
        GC.SuppressFinalize(this);
    }
}