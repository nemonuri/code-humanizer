namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public static partial class CSharpSyntaxRewritingTheory
{
    private class Binder
    {
        public SyntaxNode Node { get; }
        public Binder? Next { get; }

        public Binder(SyntaxNode node, Binder? next)
        {
            Node = node;
            Next = next;
        }
    }

    private class BoundNode
    {
        public SyntaxNode Node { get; }
        public ImmutableArray<BoundNode> Children { get; }

        public BoundNode(SyntaxNode node, IEnumerable<BoundNode>? children)
        {
            Node = node;
            Children = [..children ?? []];
        }
    }
}