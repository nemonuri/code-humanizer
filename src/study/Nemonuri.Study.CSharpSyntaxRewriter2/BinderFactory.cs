namespace Nemonuri.Study.CSharpSyntaxRewriter2;

public class BinderFactory
{
    private readonly Dictionary<SyntaxNode, Binder> _binderMap = new();

    public SyntaxNodePredicateFactory ChildPredicateFactory { get; }

    public BinderFactory(SyntaxNodePredicateFactory childPredicateFactory)
    {
        ChildPredicateFactory = childPredicateFactory;
    }

    public IReadOnlyDictionary<SyntaxNode, Binder> BinderMap => _binderMap;

    public Binder CreateOrGetBinder
    (
        SyntaxNode syntax,
        Binder? nextBinder
    )
    {
        if (!_binderMap.TryGetValue(syntax, out Binder? binder))
        {
            binder = new Binder(this, syntax, nextBinder, ChildPredicateFactory(syntax));
            _binderMap.Add(syntax, binder);
        }
        return binder;
    }

    public bool RemoveBinder(Binder binder)
    {
        return _binderMap.Remove(binder.Node);
    }
}
