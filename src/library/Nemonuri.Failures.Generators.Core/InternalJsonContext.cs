using System.Text.Json.Serialization;

namespace Nemonuri.Failures.Generators;

[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    GenerationMode = JsonSourceGenerationMode.Serialization,
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