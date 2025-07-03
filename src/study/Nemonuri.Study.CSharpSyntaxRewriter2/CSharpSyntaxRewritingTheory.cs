namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public static partial class CSharpSyntaxRewritingTheory
{
    public static CSharpSyntaxTree ConvertComplexArgumentExpressionsToLocalDeclareStatements(CSharpSyntaxTree tree)
    {
        Dictionary<SyntaxNode, Binder> binderMap = new();
        BindRoot(tree.GetRoot(), binderMap);
    }

    private static BoundNode BindRoot(SyntaxNode rootSyntax, Dictionary<SyntaxNode, Binder> binderMap)
    {
        Binder rootBinder = GetOrCreateBinder(rootSyntax, null, binderMap);

        var blockSyntaxes = rootSyntax.DescendantNodes(static n => n is not BlockSyntax).OfType<BlockSyntax>();
        return new BoundNode(rootSyntax, blockSyntaxes.Select(n => BindBlockSyntax(n, rootBinder, binderMap)));
    }

    private static BoundNode BindBlockSyntax(BlockSyntax blockSyntax, Binder nextBinder, Dictionary<SyntaxNode, Binder> binderMap)
    {
        Binder blockBinder = GetOrCreateBinder(blockSyntax, nextBinder, binderMap);

        var argumentListSyntaxes = blockSyntax.DescendantNodes(static n => n is not ArgumentListSyntax).OfType<ArgumentListSyntax>();
        return new BoundNode(blockSyntax, argumentListSyntaxes.Select(n => BindArgumentListSyntax(n, nextBinder, binderMap)));
    }

    private static BoundNode BindArgumentListSyntax(ArgumentListSyntax argumentListSyntax, Binder nextBinder, Dictionary<SyntaxNode, Binder> binderMap)
    {
        Binder argumentListBinder = GetOrCreateBinder(argumentListSyntax, nextBinder, binderMap);

        var argumentSyntaxs = argumentListSyntax.DescendantNodes(static n => n is not ArgumentSyntax).OfType<ArgumentSyntax>();
        return new BoundNode(argumentListSyntax, argumentSyntaxs.Select(n => BindArgumentSyntax(n, nextBinder, binderMap)));
    }

    private static BoundNode BindArgumentSyntax(ArgumentSyntax argumentSyntax, Binder nextBinder, Dictionary<SyntaxNode, Binder> binderMap)
    {
        Binder argumentBinder = GetOrCreateBinder(argumentSyntax, nextBinder, binderMap);

        var blockSyntaxes = argumentSyntax.DescendantNodes(static n => n is not BlockSyntax).OfType<BlockSyntax>();
        return new BoundNode(argumentSyntax, blockSyntaxes.Select(n => BindBlockSyntax(n, nextBinder, binderMap)));
    }


    private static Binder GetOrCreateBinder(SyntaxNode syntax, Binder? nextBinder, Dictionary<SyntaxNode, Binder> binderMap)
    {
        if (!binderMap.TryGetValue(syntax, out Binder? binder))
        {
            binder = new Binder(syntax, nextBinder);
            binderMap.Add(syntax, binder);
        }

        return binder;
    }
}

