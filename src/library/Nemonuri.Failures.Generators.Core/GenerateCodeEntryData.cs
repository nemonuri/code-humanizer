using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;

namespace Nemonuri.Failures.Generators;

public class GenerateCodeEntryData
{
    [JsonConstructor]
    public GenerateCodeEntryData() { }

    public IEnumerable<string>? Usings { get; set; }
    public required string Namespace { get; set; }
    public string? RootClass { get; set; }
    public required string MethodAlias { get; set; }
    public required string ValueType { get; set; }
    public IEnumerable<FailSlot>? FailSlots { get; set; }

    public bool TryGenerateCode
    (
        [NotNullWhen(true)] out string? generatedCode
    )
    {
        generatedCode = default;

        if (string.IsNullOrWhiteSpace(Namespace)) { return false; }
        if (string.IsNullOrWhiteSpace(MethodAlias)) { return false; }
        if (string.IsNullOrWhiteSpace(ValueType)) { return false; }

        generatedCode = GeneratingTheory.GenerateCode
        (
            usings: Usings,
            @namespace: Namespace,
            rootClass: RootClass,
            methodAlias: MethodAlias,
            valueType: ValueType,
            failSlots: FailSlots
        );
        return true;
    }

    public static GenerateCodeEntryData Parse(string jsonText)
    {
        var result = JsonSerializer.Deserialize<GenerateCodeEntryData>(jsonText, InternalJsonContext.Default.Options);
        Guard.IsNotNull(result);
        return result;
    }
}
