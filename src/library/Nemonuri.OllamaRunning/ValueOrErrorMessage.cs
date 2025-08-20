using SumSharp;

namespace Nemonuri.OllamaRunning;

[UnionCase("Value", nameof(TValue))]
[UnionCase("ErrorMessage", typeof(string))]
public partial class ValueOrErrorMessage<TValue>
{ }