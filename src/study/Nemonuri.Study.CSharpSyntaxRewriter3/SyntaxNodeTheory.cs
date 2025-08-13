namespace Nemonuri.Study.CSharpSyntaxRewriter3;

internal static class SyntaxNodeTheory
{
    public static bool IsArgumentSyntaxAndHasComplexExpression
    (this SyntaxNode syntaxNode)
    {
        return
            (syntaxNode is ArgumentSyntax argument) &&
            (!(
                argument.Expression is
                IdentifierNameSyntax or
                LiteralExpressionSyntax or
                RangeExpressionSyntax or
                RefExpressionSyntax or
                DefaultExpressionSyntax or
                DeclarationExpressionSyntax
            ));
    }

    public static (SyntaxNode?, int) FindAncestorOrSelf
    (
        this SyntaxNode syntaxNode,
        Func<SyntaxNode, bool> predicate
    )
    {
        Debug.Assert(syntaxNode is not null);
        Debug.Assert(predicate is not null);

        int distanceFromSelf = 0;
        for (SyntaxNode? node = syntaxNode; node is not null; node = node.Parent)
        {
            if (predicate(node))
            {
                return (node, distanceFromSelf);
            }
            distanceFromSelf++;
        }

        return (default, -1);
    }
}
