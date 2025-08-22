
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
        Guard.IsNotNullOrEmpty(failSlot.Type);

        return
$$"""
{{indentation}}public static {{internalClass}} CreateAs{{failSlot.Name}}
{{indentation}}({{failSlot.Type}} value, string message = "") =>
{{indentation}}    new(FailureTheory.Create(FailInfo.{{failSlot.Name}}(value), message));

""";
    }
}