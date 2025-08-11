using System.Collections;

namespace Nemonuri.Trees.Indexes;

public class IndexSequence : IReadOnlyList<int>
{
    private readonly ImmutableList<int> _internalList;

    public IndexSequence(ImmutableList<int> internalList)
    {
        _internalList = internalList;
    }

    public int this[int index] => _internalList[index];

    public int Count => _internalList.Count;

    public IEnumerator<int> GetEnumerator()
    {
        return _internalList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
