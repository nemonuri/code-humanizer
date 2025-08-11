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
        Assert.Equal([.. actual], expectedAsArray);
    }

    public static TheoryData<int[], int[], int[]> UpdateWhenInserted_Data => new()
    {
        { [0, 0, 3], [0], [1, 0, 3] },
        { [0, 0, 3], [0, 0], [0, 1, 3] },
        { [0, 0, 3], [0, 0, 2], [0, 0, 4] },
        { [0, 0, 3], [0, 0, 4], [0, 0, 3] },
        { [3, 6, 2, 7, 5], [3, 6, 2, 4], [3, 6, 2, 8, 5] },
        { [3, 6, 2, 7, 5], [3, 5, 2, 4], [3, 6, 2, 7, 5] },
        { [3, 6, 2, 7, 5], [3, 6, 2, 7, 5], [3, 6, 2, 7, 6] },
        { [3, 6, 2, 7, 5], [3, 6, 2, 7, 5, 0], [3, 6, 2, 7, 5] },
    };
}
