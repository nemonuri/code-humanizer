using Nemonuri.Trees.Indexes;
using System.Diagnostics;

public class SyntaxNodeInfo
{
    public SyntaxNodeInfo(SyntaxNode syntaxNode, IndexSequence indexSequence)
    {
        Debug.Assert(syntaxNode is not null);
        Debug.Assert(indexSequence is not null);

        SyntaxNode = syntaxNode;
        IndexSequence = indexSequence;
    }

    public SyntaxNode SyntaxNode { get; }
    public IndexSequence IndexSequence { get; }
}