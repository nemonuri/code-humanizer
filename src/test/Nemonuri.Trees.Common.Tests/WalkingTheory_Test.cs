using Nemonuri.Trees.RoseNodes;

namespace Nemonuri.Trees.Common.Tests;

public class WalkingTheory_Test
{
    public static TheoryData<RoseNode<int>, bool> ForallData => new()
    {
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(1, [1, 2, 3]), true},
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(2, [0, 4, 9, 11]), true},
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(-1, [3, 9, 2]), false},
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(2, [0, 4, -9, 11]), false},
        {
            new (1, [new(1), new RoseNode<int>(3).WithChildrenValues([6, 11, 8]), new(0)]),
            true
        },
        {
            N(0).WithChildren
            (
                N(1).WithChildren
                (
                    N(4),
                    N(5)
                ),
                N(2).WithChildrenValues
                (
                    7,
                    -1,
                    8
                ),
                N(3)
            ),
            false
        }
    };

    private static RoseNode<int> N(int value) => new(value);
    
    [Theory]
    [MemberData(nameof(ForallData))]
    public void TryWalkAsRoot_WhenBaseOnCreateForallPremiseUsingOptionalAggregator
    (
        RoseNode<int> roseNode, bool expected
    )
    {
        // Arrange
        RoseNodePremise<int> roseNodePremise = new();
        AdHocRoseNodeAggregatingPremise<int, bool> aggregatingPremise =
            RoseNodeAggregatingPremiseTestTheory.CreateForallPremiseUsingOptionalAggregator
            (
                roseNodePremise,
                static i => i >= 0
            );

        // Act
        bool success = WalkingTheory.TryWalkAsRoot(aggregatingPremise, roseNodePremise, roseNode, out var walkedValue);

        // Assert
        Assert.True(success);
        Assert.Equal(expected, walkedValue);
    }
}