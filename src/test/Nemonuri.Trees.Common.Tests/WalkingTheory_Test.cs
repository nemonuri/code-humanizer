using Nemonuri.Trees.RoseNodes;

namespace Nemonuri.Trees.Common.Tests;

public class WalkingTheory_Test
{
    private readonly ITestOutputHelper _output;

    public WalkingTheory_Test(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(ForallData))]
    public void TryWalkAsRoot_WhenBaseOnCreateForallPremiseUsingTryAggregator
    (
        RoseNode<int> roseNode, bool expected
    )
    {
        // Arrange
        RoseNodePremise<int> roseNodePremise = new();
        AdHocRoseNodeAggregatingPremise<int, bool> aggregatingPremise =
            RoseNodeAggregatingPremiseTestTheory.CreateForallPremiseUsingTryAggregator
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

    public static TheoryData<RoseNode<int>, bool> ForallData => new()
    {
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(1, [1, 2, 3]), true},
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(2, [0, 4, 9, 11]), true},
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(-1, [3, 9, 2]), false},
        { RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(2, [0, 4, -9, 11]), false},
    };

}