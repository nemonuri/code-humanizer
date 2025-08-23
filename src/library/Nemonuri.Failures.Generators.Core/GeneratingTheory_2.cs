
using CommunityToolkit.Diagnostics;

namespace Nemonuri.Failures.Generators;

public static partial class GeneratingTheory
{
    private static string ToCreateMethodExpression
    (
        this FailSlot failSlot,
        string internalClass,
        string indentation
    )
    {
        Guard.IsNotNullOrEmpty(failSlot.Name);

        static string CreateValuePart(string? type)
        {
            return type is { } v ?
                $"{type} value, " : "";
        }

        return
$$"""
{{indentation}}public static {{internalClass}} CreateAs{{failSlot.Name}}
{{indentation}}({{CreateValuePart(failSlot.Type)}}string message = "") =>
{{indentation}}    new(FailureTheory.Create(FailInfo.{{failSlot.Name}}(value), message));

""";
    }
}