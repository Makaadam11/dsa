using Dsa;

namespace Dsa.Tests;

public class TraversalTests
{
    private static Graph<int> BuildDirectedTree()
    {
        var graph = new Graph<int>(directed: true);
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(2, 4);
        graph.AddEdge(2, 5);
        graph.AddEdge(3, 6);
        return graph;
    }

    [Fact]
    public void DfsRecursive_VisitsInPreOrder()
        => Assert.Equal([1, 2, 4, 5, 3, 6], Traversals.DfsRecursive(BuildDirectedTree(), 1));

    [Fact]
    public void DfsIterative_MatchesRecursiveOrder()
        => Assert.Equal([1, 2, 4, 5, 3, 6], Traversals.DfsIterative(BuildDirectedTree(), 1));

    [Fact]
    public void Bfs_VisitsLevelByLevel()
        => Assert.Equal([1, 2, 3, 4, 5, 6], Traversals.Bfs(BuildDirectedTree(), 1));

    [Fact]
    public void UndirectedGraph_AddsBothDirections()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);

        Assert.Equal([2], graph.Neighbours(1));
        Assert.Equal([1], graph.Neighbours(2));
    }

    [Fact]
    public void WeightedGraph_Undirected_AddsReverseEdgeFromSource()
    {
        var graph = new WeightedGraph<string>();
        graph.AddEdge("a", "b", 7);

        Assert.Equal([("b", 7)], graph.Neighbours("a"));
        Assert.Equal([("a", 7)], graph.Neighbours("b"));
    }

    [Fact]
    public void BfsGrid_SkipsWallsAndMeasuresDistance()
    {
        int[][] grid =
        [
            [0, 0, 0],
            [0, -1, 0],
            [0, 0, 0],
        ];

        var visited = Traversals.BfsGrid(grid, 0, 0);

        Assert.Equal(8, visited.Count);
        Assert.DoesNotContain(visited, cell => cell is { Row: 1, Col: 1 });
        Assert.Equal(4, visited.Single(c => c is { Row: 2, Col: 2 }).Distance);
    }
}
