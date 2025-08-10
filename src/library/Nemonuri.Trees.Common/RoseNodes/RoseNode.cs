namespace Nemonuri.Trees.RoseNodes;

public class RoseNode<T>
{
    public T? Value { get; }

    public ImmutableArray<RoseNode<T>> Children { get; }

    public RoseNode(T? value, IEnumerable<RoseNode<T>>? children)
    {
        Value = value;
        Children = children?.ToImmutableArray() ?? [];
    }

    public RoseNode(T? value) : this(value, default)
    { }
}
