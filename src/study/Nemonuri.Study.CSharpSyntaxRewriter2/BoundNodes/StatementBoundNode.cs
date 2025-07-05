
namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class StatementBoundNode : IBoundNode<StatementSyntax>,
    IChildBoundNodeProvider<ArgumentListBoundNode>,
    IParentBoundNodeProvider<BlockBoundNode>
{
    public StatementBoundNode(StatementSyntax syntax, BlockBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;
        ChildBoundNodes = Syntax
            .GetDescendantNodes<ArgumentListSyntax>()
            .Select(n => new ArgumentListBoundNode(n, this))
            .ToImmutableArray();
    }

    public BlockBoundNode ParentBoundNode { get; }

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    public IReadOnlyList<ArgumentListBoundNode> ChildBoundNodes { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;

    public StatementSyntax Syntax { get; }


    SyntaxNode IBoundNode.Syntax => Syntax;
}
