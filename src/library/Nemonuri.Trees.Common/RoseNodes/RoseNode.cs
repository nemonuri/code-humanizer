namespace Nemonuri.Trees.RoseNodes;

public class RoseNode<T>
{
    public T? Value { get; }

    public ImmutableArray<RoseNode<T>> Children { get; }

    public RoseNode(T? value, ImmutableArray<RoseNode<T>> children)
    {
        Value = value;
        Children = children;
    }

    /*
    public RoseNode(T? value, IEnumerable<RoseNode<T>>? children)
    : this(value, children?.ToImmutableArray() ?? [])
    { }
    */

    public RoseNode(T? value) : this(value, ImmutableArray<RoseNode<T>>.Empty)
    { }

    /*
        public RoseNode(T? value, IEnumerable<T>? values)
        : this(value, values?.Select(static a => new RoseNode<T>(a)))
        { }
    */
    public RoseNode<T> WithValue(T? value) => new(value, Children);

    public RoseNode<T> WithChildren(params ImmutableArray<RoseNode<T>> children) => new(Value, children);

    public RoseNode<T> WithChildrenValues(params IEnumerable<T> childrenValues) =>
        WithChildren([.. childrenValues.Select(static a => new RoseNode<T>(a))]);
}
