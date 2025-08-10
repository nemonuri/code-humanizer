namespace Nemonuri.Trees;

public class RoseNodePremise<T> : IRoseNodePremise<T, RoseNode<T>>
{
    public RoseNodePremise() { }

    public T? GetValue(RoseNode<T> source)
    {
        Debug.Assert(source is not null);
        return source.Value;
    }

    public IEnumerable<RoseNode<T>> GetChildren(RoseNode<T> source)
    {
        Debug.Assert(source is not null);
        return source.Children;
    }
}