
using SumSharp;

namespace Nemonuri.Failures;

[UnionCase("Value", nameof(TValue))]
[UnionCase("Failure", "Failure<TFailureData>")]
public partial class ValueOrFailure<TValue, TFailureData>
{
}
