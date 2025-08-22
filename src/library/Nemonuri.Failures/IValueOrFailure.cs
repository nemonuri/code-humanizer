using System.Diagnostics;

namespace Nemonuri.Failures;

public interface IValueOrFailure<TValue, TFailInfo>
{
    bool IsValue { get; }
    bool IsFailure { get; }
    TValue GetValue();
    Failure<TFailInfo> GetFailure();
}
