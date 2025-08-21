
using SumSharp;

namespace Nemonuri.Failures;

[UnionCase("Value", nameof(TValue))]
[UnionCase("Failure", "Failure<TFailInfo>")]
public partial class ValueOrFailure<TValue, TFailInfo>
{
}
