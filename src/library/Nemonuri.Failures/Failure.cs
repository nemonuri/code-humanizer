using CommunityToolkit.Diagnostics;

namespace Nemonuri.Failures;

public class Failure<TData>
{
    public Failure(TData data, string message)
    {
        Guard.IsNotNull(data);
        Guard.IsNotNull(message);

        Data = data;
        Message = message;
    }

    public TData Data { get; }

    public string Message { get; }
}
