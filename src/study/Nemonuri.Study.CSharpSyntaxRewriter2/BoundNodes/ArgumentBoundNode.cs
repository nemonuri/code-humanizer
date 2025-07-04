
namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class ArgumentBoundNode : IBoundNode<ArgumentSyntax>,
    IChildBoundNodeProvider<BlockBoundNode>,
    IParentBoundNodeProvider<ArgumentListBoundNode>
{
    public ArgumentBoundNode(ArgumentSyntax syntax, ArgumentListBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is BlockSyntax)
            .OfType<BlockSyntax>()
            .Select(n => new BlockBoundNode(n, this))
            .ToImmutableArray();
    }

    public ArgumentListBoundNode ParentBoundNode { get; }

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    public ImmutableArray<BlockBoundNode> ChildBoundNodes { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;

    public ArgumentSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;
}
