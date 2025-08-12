namespace Nemonuri.Study.CSharpSyntaxRewriter3;

internal static class SyntaxNodeTheory
{
    public static bool IsArgumentSyntaxAndHasComplexExpression
    (this SyntaxNode syntaxNode)
    {
        return
            (syntaxNode is ArgumentSyntax argument) &&
            (!(argument.Expression is IdentifierNameSyntax or LiteralExpressionSyntax));
    }
}
