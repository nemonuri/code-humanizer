namespace Nemonuri.Trees.Indexes.Tests;

public class IndexSequenceTheory_Test
{
    [Theory]
    [MemberData(nameof(UpdateWhenInserted_Data))]
    public void UpdateWhenInserted
    (
        int[] sourceAsArray,
        int[] insertedAsArray,
        int[] expectedAsArray
    )
    {
        // Arrange
        IndexSequence source = [.. sourceAsArray];
        IndexSequence inserted = [.. insertedAsArray];

        Assert.False(inserted.IsReferencingRoot);

        // Act
        IndexSequence actual = source.UpdateAsInserted(inserted);

        // Assert
        Assert.Equal(expectedAsArray.AsSpan(), [.. actual]);
    }

    public static TheoryData<int[], int[], int[]> UpdateWhenInserted_Data => new()
    {
        // (p0 /\ p1 /\ p2) ==> source <> result
        { [0, 0, 3], [0], [1, 0, 3] },
        { [0, 0, 3], [0, 0], [0, 1, 3] },
        { [0, 0, 3], [0, 0, 2], [0, 0, 4] },
        { [3, 6, 2, 7, 5], [3, 6, 2, 4], [3, 6, 2, 8, 5] },
        { [3, 6, 2, 7, 5], [3, 6, 2, 7, 5], [3, 6, 2, 7, 6] },

        // ~p2 ==> source == result
        { [0, 0, 3], [0, 0, 4], [0, 0, 3] },

        // ~p1 ==> source == result
        { [3, 6, 2, 7, 5], [3, 5, 2, 4], [3, 6, 2, 7, 5] },

        // ~p0 ==> source == result
        { [3, 6, 2, 7, 5], [3, 6, 2, 7, 5, 0], [3, 6, 2, 7, 5] },
    };

    [Theory]
    [MemberData(nameof(TryGetBoundInSubtree_Data))]
    public void TryGetBoundInSubtree
    (
        int[] sourceAsArray,
        int[] subtreeRootAsArray,
        bool expectedSuccess,
        int[] expectedBound
    )
    {
        // Arrange
        IndexSequence source = [.. sourceAsArray];
        IndexSequence subtreeRoot = [.. subtreeRootAsArray];

        // Act
        bool actualSuccess = source.TryGetBoundInSubtree(subtreeRoot, out var actualBound);

        // Assert
        Assert.Equal(expectedSuccess, actualSuccess);
        Assert.Equal(expectedBound.AsSpan(), [.. actualBound ?? []]);
    }

    public static TheoryData<int[], int[], bool, int[]> TryGetBoundInSubtree_Data => new()
    {
        // (~p0 /\ p1 /\ p2) ==> success /\ (result <> source)
        { [1, 2, 3, 4, 5], [1, 2, 3], true, [4, 5] },
        { [2, 2, 5, 5], [2], true, [2,5,5]  },
        { [2, 2, 5, 5], [2, 2, 5, 5], true, [] },

        // p0 ==> success /\ (result == source)
        { [43, 2, 764], [], true, [43, 2, 764] },

        // ~p1 ==> ~success
        { [43, 2, 764], [43, 2, 764, 4], false, [] },

        // ~p2 ==> ~success
        { [1, 2, 3, 4, 5], [1, 2, 2], false, [] },
        { [2, 2, 5, 5], [2, 5], false, [] },
    };

    [Theory]
    [MemberData(nameof(UpdateAsRemoved_Data))]
    public void UpdateAsRemoved
    (
        int[] sourceAsArray,
        int[] removedAsArray,
        bool expectedBoundInRemovedSubtree,
        int[] expectedIndexSequence
    )
    {
        // Arrange
        IndexSequence source = [.. sourceAsArray];
        IndexSequence removed = [.. removedAsArray];

        // Act
        var actual = source.UpdateAsRemoved(removed);

        // Assert
        Assert.Equal(expectedBoundInRemovedSubtree, actual.BoundInRemovedSubtree);
        Assert.Equal(expectedIndexSequence.AsSpan(), [.. actual.IndexSequence]);
    }

    public static TheoryData<int[], int[], bool, int[]> UpdateAsRemoved_Data => new()
    {
        // (~p3 /\ p0 /\ p1 /\ p2) ==> (~boundInRemovedSubtree /\ (indexSequence <> source))
        { [1, 2, 4], [1, 1], false, [1, 1, 4] },
        { [1, 2, 4], [1, 2, 3], false, [1, 2, 3] },

        // p3 ==> (boundInRemovedSubtree)
        { [1, 2, 4], [1,2], true, [4] },
        { [1, 2, 4], [1,2,4], true, [] },
        { [1, 2, 4], [], true, [1,2,4] },

        // ~p0 ==> (~boundInRemovedSubtree /\ (indexSequence == source))
        { [1, 2, 4], [1,2,4,5], false, [1,2,4] },

        // ~p2 ==> (~boundInRemovedSubtree /\ (indexSequence == source))
        { [1, 2, 4], [1,2,5], false, [1,2,4] },
    };
}
