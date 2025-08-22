using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.Failures.Generators;

public record FailSlot
{
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