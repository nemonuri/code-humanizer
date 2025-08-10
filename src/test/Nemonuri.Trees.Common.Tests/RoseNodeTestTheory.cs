using Nemonuri.Trees.RoseNodes;

namespace Nemonuri.Trees.Common.Tests;

public static class RoseNodeTestTheory
{
    public static RoseNode<T> CreateFromNodeValueAndChildrenValues<T>
    (
        T? nodeValue,
        IEnumerable<T>? childrenValues
    )
    { 
        return new(nodeValue, [.. childrenValues?.Select(a => new RoseNode<T>(a))??[]]);
    }
}