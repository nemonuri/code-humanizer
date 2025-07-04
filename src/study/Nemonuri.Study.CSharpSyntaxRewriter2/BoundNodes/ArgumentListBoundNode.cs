
namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class ArgumentListBoundNode : IBoundNode<ArgumentListSyntax>,
    IChildBoundNodeProvider<ArgumentBoundNode>,
    IParentBoundNodeProvider<StatementBoundNode>
{
    public ArgumentListBoundNode(ArgumentListSyntax syntax, StatementBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;
        ChildBoundNodes = Syntax
            .GetDescendantNodes(static n => n is ArgumentSyntax)
            .OfType<ArgumentSyntax>()
            .Select(n => new ArgumentBoundNode(n, this))
            .ToImmutableArray();
    }

    public StatementBoundNode ParentBoundNode { get; }

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    public ImmutableArray<ArgumentBoundNode> ChildBoundNodes { get; }

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;

    public ArgumentListSyntax Syntax { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;
}
