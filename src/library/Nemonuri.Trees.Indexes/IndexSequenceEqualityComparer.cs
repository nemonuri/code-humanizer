
namespace Nemonuri.Trees.Indexes;

public class IndexSequenceEqualityComparer : IEqualityComparer<IndexSequence>
{
    public IndexSequenceEqualityComparer() { }

    public bool Equals(IndexSequence? x, IndexSequence? y) =>
        Int32ReadOnlyListCompareTheory.AreEqual(x, y);

    public int GetHashCode([DisallowNull] IndexSequence obj) =>
        Int32ReadOnlyListCompareTheory.CalculateHashCode(obj);
}