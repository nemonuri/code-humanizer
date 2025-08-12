namespace Nemonuri.Trees.Indexes.Tests;

public class Int32ReadOnlyListCompareTheory_Test
{
    [Theory]
    [MemberData(nameof(AreEqual_Data))]
    public void AreEqual
    (
        int[]? x,
        int[]? y,
        bool expected
    )
    {
        // Arrage

        // Act
        bool actual = Int32ReadOnlyListCompareTheory.AreEqual(x, y);

        // Assert
        Assert.Equal(expected, actual);
    }

    public static TheoryData<int[]?, int[]?, bool> AreEqual_Data => new()
    {
        { null, null, false },
        { null, [1,4,8], false },
        { [37,2,85,0], [37,2,85,0], true },
        { [], [], true },
        { [37,2,85,0], [37,85,2,0], false },
        { [37,2,85,0], [37,85,0], false },
        { [5], [], false },
    };

    [Theory]
    [MemberData(nameof(CalculateHashCode_Data))]
    public void CalculateHashCode_HashCodesOfOriginalAndToListResultShouldEqual
    (int[] source)
    {
        // Arrage
        List<int> toListResult = source.ToList();

        // Act
        int originalHashCode = Int32ReadOnlyListCompareTheory.CalculateHashCode(source);
        int toListResultHashCode = Int32ReadOnlyListCompareTheory.CalculateHashCode(toListResult);

        // Assert
        Assert.True(originalHashCode == toListResultHashCode);
    }

    public static TheoryData<int[]> CalculateHashCode_Data => new()
    {
        { [] },
        { [1,4,5,8] },
        { [2,3,0,0,0,9,8,1] },
    };
}