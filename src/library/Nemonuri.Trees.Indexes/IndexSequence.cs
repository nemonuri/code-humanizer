using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

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

    public IndexSequence Slice(int start, int length)
    {
        Guard.IsInRange(start, 0, Count + 1);
        Guard.IsGreaterThanOrEqualTo(length, 0);
        Guard.IsLessThanOrEqualTo(start + length, Count);

        var builder = ImmutableList<int>.Empty.ToBuilder();
        for (int i = 0; i < length; i++)
        {
            builder.Add(_internalList[start + i]);
        }

        return new IndexSequence(builder.ToImmutable());
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder()
        .Append(nameof(IndexSequence))
        .Append(" { ");
        if (PrintMembers(sb))
        {
            sb.Append(' ');
        }
        sb.Append('}');
        return sb.ToString();
    }

    public bool PrintInternalList(StringBuilder stringBuilder)
    {
        stringBuilder
        .Append('[').AppendJoin(',', _internalList).Append(']');
        return true;
    }

    public bool PrintMembers(StringBuilder stringBuilder)
    {
        stringBuilder.Append("InternalList = ");
        if (PrintInternalList(stringBuilder))
        {
            stringBuilder.Append(", ");
        }
        stringBuilder.Append($"{nameof(Count)} = {Count}");
        return true;
    }

    public bool CanPop() => _internalList.Count > 0;

    public IndexSequence Pop()
    {
        Guard.IsGreaterThan(Count, 0);
        return new(_internalList.RemoveAt(_internalList.Count - 1));
    }
}
