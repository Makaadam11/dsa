namespace Dsa;

public static class Traversals
{
    public static List<T> DfsRecursive<T>(Graph<T> graph, T start) where T : notnull
    {
        var visited = new HashSet<T>();
        var result = new List<T>();
        DfsRecursive(graph, start, visited, result);
        return result;
    }

    private static void DfsRecursive<T>(Graph<T> graph, T node, HashSet<T> visited, List<T> result)
        where T : notnull
    {
        visited.Add(node);
        result.Add(node);

        foreach (var neighbour in graph.Neighbours(node))
        {
            if (!visited.Contains(neighbour))
                DfsRecursive(graph, neighbour, visited, result);
        }
    }

    public static List<T> DfsIterative<T>(Graph<T> graph, T start) where T : notnull
    {
        var result = new List<T>();
        var stack = new Stack<T>();
        var visited = new HashSet<T>();

        stack.Push(start);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (!visited.Add(node))
                continue;

            result.Add(node);

            var neighbours = graph.Neighbours(node);
            for (var i = neighbours.Count - 1; i >= 0; i--)
            {
                if (!visited.Contains(neighbours[i]))
                    stack.Push(neighbours[i]);
            }
        }

        return result;
    }

    public static List<T> Bfs<T>(Graph<T> graph, T start) where T : notnull
    {
        var queue = new Queue<T>();
        var visited = new HashSet<T> { start };
        var result = new List<T>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            result.Add(node);

            foreach (var neighbour in graph.Neighbours(node))
            {
                if (visited.Add(neighbour))
                    queue.Enqueue(neighbour);
            }
        }

        return result;
    }

    public static List<(int Row, int Col, int Distance)> BfsGrid(int[][] grid, int startRow, int startCol)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        ReadOnlySpan<(int Dr, int Dc)> directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

        var visited = new HashSet<(int, int)> { (startRow, startCol) };
        var queue = new Queue<(int Row, int Col, int Distance)>();
        var result = new List<(int Row, int Col, int Distance)>();

        queue.Enqueue((startRow, startCol, 0));

        while (queue.Count > 0)
        {
            var (r, c, dist) = queue.Dequeue();
            result.Add((r, c, dist));

            foreach (var (dr, dc) in directions)
            {
                var nr = r + dr;
                var nc = c + dc;

                if (nr < 0 || nr >= rows || nc < 0 || nc >= cols)
                    continue;

                // -1 to ściana; zależnie od zadania próg może być inny
                if (grid[nr][nc] == -1)
                    continue;

                if (!visited.Add((nr, nc)))
                    continue;

                queue.Enqueue((nr, nc, dist + 1));
            }
        }

        return result;
    }
}
