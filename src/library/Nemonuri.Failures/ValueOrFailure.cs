
using SumSharp;

namespace Nemonuri.Failures;

[UnionCase("Value", nameof(TValue))]
[UnionCase("Failure", "Failure<TFailInfo>")]
public partial class ValueOrFailure<TValue, TFailInfo> :
    IValueOrFailure<TValue, TFailInfo>
{
    public TValue GetValue() => AsValue;
    public Failure<TFailInfo> GetFailure() => AsFailure;
}
