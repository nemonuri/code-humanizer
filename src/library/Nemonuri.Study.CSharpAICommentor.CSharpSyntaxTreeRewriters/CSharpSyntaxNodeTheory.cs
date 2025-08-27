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
}

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
}