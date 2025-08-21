using SumSharp;

namespace Nemonuri.OllamaRunning;

[UnionCase("Value", nameof(TValue))]
[UnionCase("ErrorMessage", typeof(string))]
public partial class DisposableValueOrErrorMessage<TValue> : IDisposable
    where TValue : IDisposable
{ 
    public void Dispose()
    {
        if (AsValue is { } ensuredValue)
        {
            ensuredValue.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}