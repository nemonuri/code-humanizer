
namespace Nemonuri.Study.CSharpAICommentor.CSharpSyntaxTreeRewriters;

public static partial class CSharpSyntaxNodeTheory
{
    public static CSharpSyntaxTree
    CreateCSharpSyntaxTreeFromText
    (
        string text,
        CancellationToken cancellationToken = default
    )
    {
        Guard.IsNotNull(text);

        var parsed = CSharpSyntaxTree.ParseText(text, cancellationToken: cancellationToken);
        return (CSharpSyntaxTree)parsed;
    }

    public static bool ContainsMissingNodeOrToken(SyntaxNodeOrToken syntaxNodeOrToken)
    {
        bool success =
        TreeNodeAggregatingTheory.TryAggregateAsRoot
        (
            Constants.s_syntaxNodeOrTokenTypedIndexedTreeNodesFromRootAggregator,
            Constants.s_isMissingExistAdHocTreeNodeAggregator,
            Constants.s_syntaxNodeOrTokenChildrenProvider,
            syntaxNodeOrToken,
            out var aggregated
        );

        return success ? aggregated : false;
    }

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
