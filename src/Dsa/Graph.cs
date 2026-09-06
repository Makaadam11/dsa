namespace Dsa;

public sealed class Graph<T> where T : notnull
{
    private readonly Dictionary<T, List<T>> _adj = [];

    public Graph(bool directed = false) => Directed = directed;

    public bool Directed { get; }

    public IReadOnlyDictionary<T, List<T>> Adjacency => _adj;

    public void AddEdge(T u, T v)
    {
        Neighbours(u).Add(v);
        if (!Directed)
            Neighbours(v).Add(u);
    }

    public List<T> Neighbours(T node)
    {
        if (!_adj.TryGetValue(node, out var list))
        {
            list = [];
            _adj[node] = list;
        }

        return list;
    }
}

public sealed class WeightedGraph<T> where T : notnull
{
    private readonly Dictionary<T, List<(T To, int Weight)>> _adj = [];

    public WeightedGraph(bool directed = false) => Directed = directed;

    public bool Directed { get; }

    public IReadOnlyDictionary<T, List<(T To, int Weight)>> Adjacency => _adj;

    public void AddEdge(T u, T v, int weight = 1)
    {
        Neighbours(u).Add((v, weight));
        if (!Directed)
            Neighbours(v).Add((u, weight));
    }

    public List<(T To, int Weight)> Neighbours(T node)
    {
        if (!_adj.TryGetValue(node, out var list))
        {
            list = [];
            _adj[node] = list;
        }

        return list;
    }
}
