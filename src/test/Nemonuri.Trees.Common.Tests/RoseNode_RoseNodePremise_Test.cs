using Nemonuri.Trees.RoseNodes;

namespace Nemonuri.Trees.Common.Tests;

public class RoseNode_RoseNodePremise_Test
{
    [Theory]
    [InlineData(0, new int[] { 1, 2, 3, 5, 4 })]
    [InlineData(1, new int[] { })]
    [InlineData(-1, new int[] { 0, 1 })]
    [InlineData(10, new int[] { -1, 2, 3, 2345, 21, 0, -19, 54, 2345 })]
    public void Constructor_ValueAndChildrenShouldStructurallyEqualToSourceValueAndChildren
    (
        int nodeValue,
        int[] childrenValues
    )
    {
        // Arrange
        RoseNodePremise<int> roseNodePremise = new RoseNodePremise<int>();

        // Act
        RoseNode<int> roseNode = RoseNodeTestTheory.CreateFromNodeValueAndChildrenValues(nodeValue, childrenValues);

        // Assert
        Assert.Equal(nodeValue, roseNode.Value);
        Assert.Equal(childrenValues, roseNode.Children.Select(a => a.Value));
        Assert.Equal(nodeValue, roseNodePremise.GetValue(roseNode));
        Assert.Equal(childrenValues, roseNodePremise.GetChildren(roseNode).Select(a => a.Value));
    }
}
