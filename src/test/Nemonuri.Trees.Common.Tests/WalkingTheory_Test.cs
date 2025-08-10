using Nemonuri.Trees.RoseNodes;

namespace Nemonuri.Trees.Common.Tests;

public class WalkingTheory_Test
{
    private readonly ITestOutputHelper _output;

    public WalkingTheory_Test(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TryWalkAsRoot_ForAll()
    {
        // Arrange
        RoseNode<int> roseNode =
            RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(1, [1, 2, 3]);
        RoseNodePremise<int> roseNodePremise = new();
        AdHocRoseNodeAggregatingPremise<int, bool> aggregatingPremise = new
        (
            defaultSeedProvider: () => true,
            tryAggregator: (bool seed, WalkingNodeInfo<RoseNode<int>> source, out bool aggregated) =>
            {
                if (seed == false)
                {
                    aggregated = false;
                    return true;
                }

                var child = source.ChildAndIndex.Child;
                if (child is null) { goto Fail; }

                int value = roseNodePremise.GetValue(child);
                aggregated = (value >= 0);

                _output.WriteLine($"{nameof(TryWalkAsRoot_ForAll)}, {nameof(value)}: {value}, {nameof(aggregated)}: {aggregated}");
                return true;

            Fail:
                aggregated = false;
                return false;
            }
        );

        // Act
        bool success = WalkingTheory.TryWalkAsRoot(aggregatingPremise, roseNodePremise, roseNode, out var walkedValue);

        // Assert
        Assert.True(success);
        Assert.True(walkedValue);
    }

}