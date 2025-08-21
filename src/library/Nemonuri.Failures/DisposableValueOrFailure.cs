
using SumSharp;

namespace Nemonuri.Failures;

[UnionCase("Value", nameof(TValue))]
[UnionCase("Failure", "Failure<TFailInfo>")]
public partial class DisposableValueOrFailure<TValue, TFailInfo> : IDisposable
    where TValue : IDisposable
{
    public void Dispose()
    {
        if (AsValueOrDefault is { } value) { value.Dispose(); }
        GC.SuppressFinalize(this);
    }
}