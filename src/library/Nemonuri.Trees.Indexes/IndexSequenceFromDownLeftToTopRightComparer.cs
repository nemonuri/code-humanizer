
namespace Nemonuri.Trees.Indexes;

public class IndexSequenceFromDownLeftToTopRightComparer : IComparer<IndexSequence>
{
    public IndexSequenceFromDownLeftToTopRightComparer() { }

    public int Compare(IndexSequence? x, IndexSequence? y) =>
        Int32ReadOnlyListCompareTheory.CompareFromDownLeftToTopRight(x, y);
}
