using CommunityToolkit.Diagnostics;

namespace Nemonuri.Failures;

public class Failure<TFailInfo>
{
    public Failure(TFailInfo failInfo, string? message)
    {
        Guard.IsNotNull(failInfo);

        FailInfo = failInfo;
        Message = message ?? "";
    }

    public Failure(TFailInfo data) : this(data, null)
    { }

    public TFailInfo FailInfo { get; }

    public string Message { get; }

    public override string ToString()
    {
        return $"Failure<{typeof(TFailInfo).FullName}> {{ {nameof(FailInfo)} = {FailInfo}, {nameof(Message)} = {Message} }}";
    }
}
