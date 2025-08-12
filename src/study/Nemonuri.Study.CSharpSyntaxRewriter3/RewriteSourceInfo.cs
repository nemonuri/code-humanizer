namespace Nemonuri.Study.CSharpSyntaxRewriter3;

internal class RewriteSourceInfo
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
}