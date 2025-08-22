using System.Text.Json.Serialization;

namespace Nemonuri.Failures.Generators;

public class GenerateCodeEntryData
{
    [JsonConstructor]
    public GenerateCodeEntryData() { }

    public IEnumerable<string>? Usings { get; set; }
    public required string Namespace { get; set; }
    public string? RootClass { get; set; } = null;
    public required string MethodAlias { get; set; }
    public required string ValueType { get; set; }
    public IEnumerable<FailSlot>? FailSlots { get; set; }
}
