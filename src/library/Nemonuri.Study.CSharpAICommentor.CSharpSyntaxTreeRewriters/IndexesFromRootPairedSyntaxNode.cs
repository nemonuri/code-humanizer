
namespace Nemonuri.Study.CSharpAICommentor.CSharpSyntaxTreeRewriters;

internal readonly struct IndexesFromRootPairedSyntaxNode
{
    public IndexesFromRootPairedSyntaxNode(IndexSequence indexesFromRoot, SyntaxNode syntaxNode)
    {
        IndexesFromRoot = indexesFromRoot;
        SyntaxNode = syntaxNode;
    }

    public IndexSequence IndexesFromRoot { get; }
    public SyntaxNode SyntaxNode { get; }
}