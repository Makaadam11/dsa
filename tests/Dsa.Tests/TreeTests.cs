using Dsa;

namespace Dsa.Tests;

public class TreeTests
{
    [Fact]
    public void LevelOrder_GroupsNodesByDepth()
    {
        var root = new TreeNode<int>(
            1,
            new TreeNode<int>(2, new TreeNode<int>(4), new TreeNode<int>(5)),
            new TreeNode<int>(3, right: new TreeNode<int>(6)));

        var levels = Tree.LevelOrder(root);

        Assert.Equal(3, levels.Count);
        Assert.Equal([1], levels[0]);
        Assert.Equal([2, 3], levels[1]);
        Assert.Equal([4, 5, 6], levels[2]);
    }

    [Fact]
    public void LevelOrder_NullRoot_ReturnsEmpty()
        => Assert.Empty(Tree.LevelOrder<int>(null));
}
