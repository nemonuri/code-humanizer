namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public class BoundNode
{
    public SyntaxNode Node { get; }
    public ImmutableArray<BoundNode> Children { get; }

    public BoundNode(SyntaxNode node, IEnumerable<BoundNode>? children)
    {
        Node = node;
        Children = [..children ?? []];
    }
}