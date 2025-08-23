
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

        static (string, string) CreateTypePart(FailSlot fs)
        {
            return fs.Type is { } type ?
                ($"{type} value, ", "(value)")
                :
                ("", "");
        }

        var (segment0, segment1) = CreateTypePart(failSlot);

        return
$$"""
{{indentation}}public static {{internalClass}} CreateAs{{failSlot.Name}}
{{indentation}}({{segment0}}string message = "") =>
{{indentation}}    new(FailureTheory.Create(FailInfo.{{failSlot.Name}}{{segment1}}, message));

""";
    }
}