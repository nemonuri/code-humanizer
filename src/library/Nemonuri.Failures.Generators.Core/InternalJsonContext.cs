using System.Text.Json.Serialization;

namespace Nemonuri.Failures.Generators;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    WriteIndented = true
)]
[JsonSerializable(typeof(GenerateCodeEntryData))]
[JsonSerializable(typeof(FailSlot))]
[JsonSerializable(typeof(IEnumerable<FailSlot>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(IEnumerable<string>))]
internal partial class InternalJsonContext : JsonSerializerContext
{
}