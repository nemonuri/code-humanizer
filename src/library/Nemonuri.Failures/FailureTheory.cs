namespace Nemonuri.Failures;

public static class FailureTheory
{
    public static Failure<TFailInfo>
    Create<TFailInfo>(TFailInfo failInfo, string? message = null) =>
        new Failure<TFailInfo>(failInfo, message);
}