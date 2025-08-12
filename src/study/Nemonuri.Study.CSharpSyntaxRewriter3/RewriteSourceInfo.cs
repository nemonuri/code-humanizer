namespace Nemonuri.Study.CSharpSyntaxRewriter3;

internal class RewriteSourceInfo : IComparable<RewriteSourceInfo>
{
    public RewriteSourceInfo(SyntaxNode syntaxNode, IndexSequence indexSequence)
    {
        Debug.Assert(syntaxNode is not null);
        Debug.Assert(indexSequence is not null);

        SyntaxNode = syntaxNode;
        IndexSequence = indexSequence;
    }

    public SyntaxNode SyntaxNode { get; }
    public IndexSequence IndexSequence { get; }

    public int CompareTo(RewriteSourceInfo? other)
    {
        return Int32ReadOnlyListCompareTheory.CompareFromDownLeftToTopRight(this.IndexSequence, other?.IndexSequence);
    }
}