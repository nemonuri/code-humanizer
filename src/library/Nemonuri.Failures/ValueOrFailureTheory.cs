using System.Diagnostics;

namespace Nemonuri.Failures;

public static class ValueOrFailureTheory
{
    public static TFailInfo GetFailInfo<TValue, TFailInfo>
    (this IValueOrFailure<TValue, TFailInfo> source)
    {
        Debug.Assert(source is not null);

        return source.GetFailure().FailInfo;
    }

    public static string GetMessage<TValue, TFailInfo>
    (this IValueOrFailure<TValue, TFailInfo> source)
    {
        Debug.Assert(source is not null);

        return source.GetFailure().Message;
    }
}