using Microsoft.CodeAnalysis;

namespace Nemonuri.Study.CSharpAICommentor;

internal readonly struct IndexSequenceAndSyntaxNode
{
    public IndexSequenceAndSyntaxNode(IndexSequence indexSequence, SyntaxNode syntaxNode)
    {
        IndexSequence = indexSequence;
        SyntaxNode = syntaxNode;
    }

    public IndexSequence IndexSequence { get; }
    public SyntaxNode SyntaxNode { get; }
}