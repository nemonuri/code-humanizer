
namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class BlockBoundNode : IBoundNode<BlockSyntax>, IChildBoundNodeProvider<StatementBoundNode>, IParentBoundNodeProvider
{

    public BlockBoundNode(BlockSyntax syntax, IBoundNode parent)
    {
        Syntax = syntax;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is StatementSyntax)
            .OfType<StatementSyntax>()
            .Select(n => new StatementBoundNode(n, this))
            .ToImmutableArray();
        ParentBoundNode = parent;
    }

    public IReadOnlyList<StatementBoundNode> ChildBoundNodes { get; }

    public BlockSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;

    public IBoundNode ParentBoundNode { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;
}
