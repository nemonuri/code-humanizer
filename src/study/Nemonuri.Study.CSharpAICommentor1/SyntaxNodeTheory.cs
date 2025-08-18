namespace Nemonuri.Study.CSharpAICommentor1;


public static class SyntaxNodeTheory
{
    public static bool ContainsMissingNodeOrToken(SyntaxNodeOrToken syntaxNodeOrToken)
    {
        bool success =
        TreeNodeAggregatingTheory.TryAggregateAsRoot
        (
            rootOriginatedContextAggregator: new RootOriginatedTreeNodeWithIndexSequenceAggregator<SyntaxNodeOrToken>(),
            treeNodeAggregator: new AdHocTreeNodeAggregator<SyntaxNodeOrToken, bool>
            (
                defaultSeedProvider: static () => false,
                optionalAggregator: static (context, siblings, children, source) =>
                {
                    if (siblings || children) { return (true, true); }
                    else { return (source.IsMissing, true); }
                }
            ),
            childrenProvider: new AdHocChildrenProvider<SyntaxNodeOrToken>
            (
                static s => s.ChildNodesAndTokens()
            ),
            treeNode: syntaxNodeOrToken,
            out var aggregated
        );

        return success ? aggregated : false;
    }
}
