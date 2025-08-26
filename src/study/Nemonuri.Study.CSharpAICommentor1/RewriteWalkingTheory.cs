

namespace Nemonuri.Study.CSharpAICommentor1;


internal static class RewriteWalkingTheory
{
    private static readonly
    AdHocIndexedPathAggregatingPremise<SyntaxNodeOrToken, ImmutableList<RoseNode<RewriteSourceInfo>>>
    s_rewriteSourceInfoNodeAggregatingPremise = new
    (
        defaultSeedProvider: static () => [],
        optionalAggregator: static (siblingsSeed, childrenSeed, source) =>
        {
            if (!source.IndexedPath.TryGetLastNode(out var syntaxNodeOrToken))
            { return (default, false); }

            if (syntaxNodeOrToken.AsNode() is { } node1 && SyntaxNodeTheory.IsArgumentSyntaxAndHasComplexExpression(node1))
            {
                return CreateResult(siblingsSeed, childrenSeed, source, node1);
            }
            else if (syntaxNodeOrToken.AsNode() is BlockSyntax block)
            {
                if (childrenSeed.Count > 0)
                {
                    return CreateResult(siblingsSeed, childrenSeed, source, block);
                }
            }
            else if (syntaxNodeOrToken.AsNode() is CompilationUnitSyntax compilationUnit)
            {
                return CreateResult(siblingsSeed, childrenSeed, source, compilationUnit);
            }

            return (siblingsSeed.AddRange(childrenSeed), true);

            static (ImmutableList<RoseNode<RewriteSourceInfo>>?, bool) CreateResult
            (
                ImmutableList<RoseNode<RewriteSourceInfo>> siblingsSeed,
                ImmutableList<RoseNode<RewriteSourceInfo>> childrenSeed,
                IndexedPathWithNodePremise<SyntaxNodeOrToken> source,
                SyntaxNode syntaxNode
            )
            {
                RoseNode<RewriteSourceInfo> newNode = new(new(syntaxNode, source.IndexedPath), [.. childrenSeed]);
                return (siblingsSeed.Add(newNode), true);
            }
        }
    );

    private static readonly SyntaxNodeOrTokenChildProvider s_syntaxNodeOrTokenChildProvider = new();

    public static bool TryGetRewriteSourceInfoRoseNode
    (
        SyntaxTree tree,
        [NotNullWhen(true)] out RoseNode<RewriteSourceInfo>? rewriteSourceInfoRoseNode
    )
    {
        Debug.Assert(tree is not null);

        if
        (
            WalkingTheory.TryWalkAsRoot
            (
                s_rewriteSourceInfoNodeAggregatingPremise,
                s_syntaxNodeOrTokenChildProvider,
                tree.GetRoot(),
                out var walkedValue
            )
        )
        {
            Debug.Assert(walkedValue.Count == 1);

            rewriteSourceInfoRoseNode = walkedValue[0];
            return true;
        }
        else
        {
            rewriteSourceInfoRoseNode = default;
            return false;
        }
    }

    private static readonly RoseNodePremise<RewriteSourceInfo> s_rewriteSourceInfoRoseNodePremise = new();

    private static readonly
    AdHocRoseNodeAggregatingPremise<RewriteSourceInfo, ImmutableList<RewriteSourceInfo>>
    s_isArgumentSyntaxAndHasComplexExpressionAggregatingPremise = new
    (
        defaultSeedProvider: static () => [],
        optionalAggregator: static (siblingsSeed, childrenSeed, source) =>
        {
            if (!source.IndexedPath.TryGetLastNode(out var lastNode))
            { return (default, false); }

            if
            (
                lastNode.Value is { } node &&
                node.SyntaxNode.IsArgumentSyntaxAndHasComplexExpression()
            )
            {
                return (siblingsSeed.AddRange(childrenSeed).Add(node), true);
            }
            else
            {
                return (siblingsSeed.AddRange(childrenSeed), true);
            }
        }
    );

    public static bool TryGetSortedRewriteSourceInfos
    (
        RoseNode<RewriteSourceInfo> rewriteSourceInfoRoseNode,
        [NotNullWhen(true)] out RewriteSourceInfo[]? sortedRewriteSourceInfos
    )
    {
        if
        (
            WalkingTheory.TryWalkAsRoot
            (
                s_isArgumentSyntaxAndHasComplexExpressionAggregatingPremise,
                s_rewriteSourceInfoRoseNodePremise,
                rewriteSourceInfoRoseNode,
                out var rewriteSourceInfos
            )
        )
        {
            sortedRewriteSourceInfos = rewriteSourceInfos.Sort().ToArray();
            return true;
        }

        sortedRewriteSourceInfos = default;
        return false;
    }
}
