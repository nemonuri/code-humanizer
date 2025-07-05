namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class ForwardedLocalDeclarationStatementBoundNode : IBoundNode<LocalDeclarationStatementSyntax>,
    IParentBoundNodeProvider<BlockBoundNode>,
    IChildBoundNodeProvider<ForwardedVariableDeclaratorBoundNode>
{
    public ForwardedLocalDeclarationStatementBoundNode(LocalDeclarationStatementSyntax syntax, BlockBoundNode parent)
    {
        Syntax = syntax;
        ParentBoundNode = parent;
    }

    public LocalDeclarationStatementSyntax Syntax { get; }

    public BlockBoundNode ParentBoundNode { get; }

    public IReadOnlyList<ForwardedVariableDeclaratorBoundNode> ChildBoundNodes { get; }

    SyntaxNode IBoundNode.Syntax => Syntax;

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;
}
