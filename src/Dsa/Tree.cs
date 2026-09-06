namespace Dsa;

public sealed class TreeNode<T>(T value, TreeNode<T>? left = null, TreeNode<T>? right = null)
{
    public T Value { get; set; } = value;
    public TreeNode<T>? Left { get; set; } = left;
    public TreeNode<T>? Right { get; set; } = right;
}

public static class Tree
{
    public static List<List<T>> LevelOrder<T>(TreeNode<T>? root)
    {
        var result = new List<List<T>>();
        if (root is null)
            return result;

        var queue = new Queue<TreeNode<T>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;
            var level = new List<T>(levelSize);

            for (var i = 0; i < levelSize; i++)
            {
                var node = queue.Dequeue();
                level.Add(node.Value);

                if (node.Left is not null)
                    queue.Enqueue(node.Left);
                if (node.Right is not null)
                    queue.Enqueue(node.Right);
            }

            result.Add(level);
        }

        return result;
    }
}
