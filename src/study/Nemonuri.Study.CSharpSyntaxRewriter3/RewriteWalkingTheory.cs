namespace Nemonuri.Study.CSharpSyntaxRewriter3;


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

            if (syntaxNodeOrToken.AsNode() is ArgumentSyntax argument)
            {
                if (!(argument.Expression is IdentifierNameSyntax or LiteralExpressionSyntax))
                {
                    return CreateResult(siblingsSeed, childrenSeed, source, argument);
                }
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
                RoseNode<RewriteSourceInfo> newNode = new(new(syntaxNode, source.IndexedPath.ToIndexSequence()), [.. childrenSeed]);
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
}
