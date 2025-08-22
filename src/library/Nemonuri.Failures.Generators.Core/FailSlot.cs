namespace Nemonuri.Failures.Generators;

public record FailSlot
{
    public required string Name { get; set; }
    public string? Type { get; set; }
}