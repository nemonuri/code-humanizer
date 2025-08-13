namespace Nemonuri.Study.CSharpSyntaxRewriter3;

internal class RewriteSourceInfo : IComparable<RewriteSourceInfo>
{
    private RewriteSourceInfo
    (
        SyntaxNode syntaxNode,
        IndexedPath<SyntaxNodeOrToken> originalIndexedPath,
        IndexSequence indexSequence
    )
    { 
        Debug.Assert(syntaxNode is not null);
        Debug.Assert(originalIndexedPath.HasRoot);

        SyntaxNode = syntaxNode;
        OriginalIndexedPath = originalIndexedPath;
        IndexSequence = indexSequence;  //originalIndexedPath.ToIndexSequence();
    }

    public RewriteSourceInfo
    (
        SyntaxNode syntaxNode,
        IndexedPath<SyntaxNodeOrToken> originalIndexedPath
    ): this(syntaxNode, originalIndexedPath, originalIndexedPath.ToIndexSequence())
    {
    }

    public SyntaxNode SyntaxNode { get; }
    public IndexedPath<SyntaxNodeOrToken> OriginalIndexedPath { get; }
    public IndexSequence IndexSequence { get; }

    public int CompareTo(RewriteSourceInfo? other)
    {
        return Int32ReadOnlyListCompareTheory.CompareFromDownLeftToTopRight(this.IndexSequence, other?.IndexSequence);
    }

    public RewriteSourceInfo WithIndexSequence(IndexSequence indexSequence)
    {
        return new(SyntaxNode, OriginalIndexedPath, indexSequence);
    }
}
