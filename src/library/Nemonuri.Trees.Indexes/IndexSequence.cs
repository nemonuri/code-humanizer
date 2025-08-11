using System.Collections;
using System.Runtime.CompilerServices;

namespace Nemonuri.Trees.Indexes;

[CollectionBuilder(typeof(IndexSequenceTheory), nameof(IndexSequenceTheory.CreateIndexSequence))]
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

    public bool IsReferencingRoot => _internalList.Count == 0;

    public IndexSequence SetItem(int index, int value)
    {
        Guard.IsInRange(index, 0, Count);

        return new IndexSequence(_internalList.SetItem(index, value));
    }
}
