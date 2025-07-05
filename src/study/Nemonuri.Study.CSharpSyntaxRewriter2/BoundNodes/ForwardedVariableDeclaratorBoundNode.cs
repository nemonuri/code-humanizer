namespace Nemonuri.Study.CSharpSyntaxRewriter2.BoundNodes;

public class ForwardedVariableDeclaratorBoundNode : IBoundNode<VariableDeclaratorSyntax>,
    IParentBoundNodeProvider<ForwardedLocalDeclarationStatementBoundNode>,
    IChildBoundNodeProvider<ComplexExpressionBoundNode>
{
    private VariableDeclaratorSyntax? _syntax;

    private ComplexExpressionBoundNode[] _childBoundNodes;

    public VariableDeclaratorSyntax Syntax => _syntax;

    public ForwardedLocalDeclarationStatementBoundNode ParentBoundNode { get; }

    public IReadOnlyList<ComplexExpressionBoundNode> ChildBoundNodes => _childBoundNodes;

    SyntaxNode IBoundNode.Syntax => Syntax;

    IBoundNode IParentBoundNodeProvider.ParentBoundNode => ParentBoundNode;

    IReadOnlyList<IBoundNode> IChildBoundNodeProvider.ChildBoundNodes => ChildBoundNodes;
}
