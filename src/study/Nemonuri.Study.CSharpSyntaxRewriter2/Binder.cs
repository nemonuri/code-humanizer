namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public class Binder
{
#if false
    public SyntaxNodePredicate ChildPredicate { get; }

    public SyntaxNode Node { get; }

    internal Binder
    (
        BinderFactory factory,
        SyntaxNode node,
        Binder? next,
        SyntaxNodePredicate childPredicate
    )
    {
        Factory = factory;
        Node = node;
        Next = next;
        ChildPredicate = childPredicate;
    }

    public BoundNode Bind()
    {
        var childSyntaxes = Node.GetDescendantNodes(ChildPredicate);
        var childBinders = childSyntaxes.Select(n => Factory.CreateOrGetBinder(n, this));
        return new BoundNode(Node, childBinders.Select(static b => b.Bind()));
    }
#endif

    internal Binder(BinderFactory factory)
    {
        Next = null;
        Factory = factory;
    }

    internal Binder(Binder next)
    {
        Next = next;
        Factory = Next.Factory;
    }

    public BinderFactory Factory { get; }
    public Binder? Next { get; }

    public IBoundNode Bind(SyntaxNode node)
    {
        throw new NotImplementedException();
    }
}

public delegate bool SyntaxNodePredicate(SyntaxNode node);

public delegate SyntaxNodePredicate SyntaxNodePredicateFactory(SyntaxNode node);