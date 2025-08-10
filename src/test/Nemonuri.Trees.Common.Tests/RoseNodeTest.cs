namespace Nemonuri.Trees.Common.Tests;

public class RoseNodeTest
{
    [Fact]
    public void Constructor()
    {
        // Arrange
        int nodeValue = 0;
        int[] childrenValues = [1, 2, 3, 5, 4];
    
        // Act
        RoseNode<int> roseNode = new(nodeValue, childrenValues.Select(a => new RoseNode<int>(a)));

        // Assert
        Assert.Equal(0, roseNode.Value);
        Assert.Equal(childrenValues, roseNode.Children.Select(a => a.Value));
    }
}
