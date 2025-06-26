using System.Text.RegularExpressions;

namespace Nemonuri.Study.CSharpSyntaxRewriter1;

internal static partial class RegexTheory
{
    [GeneratedRegex("""(\r\n)|\n""", RegexOptions.ECMAScript)]
    public static partial Regex GetLineBreakRegex();
}