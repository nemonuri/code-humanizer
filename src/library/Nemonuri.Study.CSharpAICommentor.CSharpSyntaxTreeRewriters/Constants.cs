
namespace Nemonuri.Study.CSharpAICommentor.CSharpSyntaxTreeRewriters;

internal static partial class Constants
{
    internal static readonly IndexedTreeNodesFromRootAggregator<SyntaxNodeOrToken>
    s_syntaxNodeOrTokenTypedIndexedTreeNodesFromRootAggregator = new();

    internal static readonly IChildrenProvider<SyntaxNodeOrToken>
    s_syntaxNodeOrTokenChildrenProvider = new AdHocChildrenProvider<SyntaxNodeOrToken>
    (
        static s => s.ChildNodesAndTokens()
    );

    internal static readonly AdHocTreeNodeAggregator<SyntaxNodeOrToken, bool>
    s_isMissingExistAdHocTreeNodeAggregator = new
    (
        defaultSeedProvider: static () => false,
        optionalAggregator: static (context, siblings, children, source) =>
        {
            if (siblings || children) { return (true, true); }
            else { return (source.IsMissing, true); }
        }
    );

    internal static readonly
    AdHocTreeNodeAggregator<SyntaxNodeOrToken, ImmutableList<RoseTreeNode<IndexesFromRootPairedSyntaxNode>>>
    s_syntaxNodeOrTokenToIndexesFromRootPairedSyntaxNodeTreeAggregator = new
    (
        defaultSeedProvider: static () => [],
        optionalAggregator: static (context, siblingsAggregated, childrenAggregated, source) =>
        {
            if
            (
                source.AsNode() is { } node1 &&
                (
                    node1.IsArgumentSyntaxAndHasComplexExpression() ||
                    node1 is BlockSyntax ||
                    node1 is CompilationUnitSyntax
                )
            )
            {
                RoseTreeNode<IndexesFromRootPairedSyntaxNode> newNode = new
                (
                    new IndexesFromRootPairedSyntaxNode(context.ToIndexSequence(), node1),
                    [.. childrenAggregated]
                );
                return (siblingsAggregated.Add(newNode), true);
            }

            return (siblingsAggregated.AddRange(childrenAggregated), true);
        }
    );

    internal static readonly
    IndexedRoseTreeNodesFromRootAggregator<IndexesFromRootPairedSyntaxNode>
    s_indexesFromRootPairedSyntaxNodeTreeContextAggregator = new();

    internal static readonly
    AdHocRoseTreeNodeAggregator<IndexesFromRootPairedSyntaxNode, ImmutableList<IndexSequence>>
    s_indexesFromRootPairedSyntaxNodeTreeNodeAggregator = new
    (
        defaultSeedProvider: static () => [],
        optionalAggregator: static (context, siblingsAggregated, childrenAggregated, source) =>
        {
            if
            (
                source.Value.SyntaxNode is { } node1 &&
                node1.IsArgumentSyntaxAndHasComplexExpression()
            )
            {
                return (siblingsAggregated.AddRange(childrenAggregated).Add(source.Value.IndexesFromRoot), true);
            }
            else
            {
                return (siblingsAggregated.AddRange(childrenAggregated), true);
            }
        }
    );

    internal static readonly
    RoseTreeNodeChildrenProvider<IndexesFromRootPairedSyntaxNode>
    s_indexesFromRootPairedSyntaxNodeTreeChildrenProvider = new();
}
