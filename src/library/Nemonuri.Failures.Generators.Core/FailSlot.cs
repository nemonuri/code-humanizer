using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Nemonuri.Failures.Generators;

public record FailSlot
{
    [JsonConstructor]
    public FailSlot() { }

    [SetsRequiredMembers]
    public FailSlot(string name, string? type = null)
    {
        Name = name;
        Type = type;
    }

    public required string Name { get; set; }
    public string? Type { get; set; }
}