using Dsa;

var graph = new Graph<int>(directed: true);
graph.AddEdge(1, 2);
graph.AddEdge(1, 3);
graph.AddEdge(2, 4);
graph.AddEdge(2, 5);
graph.AddEdge(3, 6);

Console.WriteLine(Format(Traversals.DfsRecursive(graph, 1)));
Console.WriteLine(Format(Traversals.DfsIterative(graph, 1)));
Console.WriteLine(Format(Traversals.Bfs(graph, 1)));

int[] arr = [1, 3, 5, 7, 9];

Console.WriteLine(Search.BinarySearch(arr, 5));   // 2
Console.WriteLine(Search.BinarySearch(arr, 4));   // -1
Console.WriteLine(Search.BinarySearch(arr, 1));   // 0
Console.WriteLine(Search.BinarySearch(arr, 9));   // 4

Console.WriteLine(Search.LowerBound(arr, 5));     // 2
Console.WriteLine(Search.LowerBound(arr, 4));     // 2 (insertion point)
Console.WriteLine(Search.LowerBound(arr, 0));     // 0
Console.WriteLine(Search.LowerBound(arr, 10));    // 5

int[] withDuplicates = [1, 3, 3, 3, 5, 7];
Console.WriteLine(Search.LowerBound(withDuplicates, 3));   // 1

Console.WriteLine(Strings.IsPalindrome("racecar"));   // True
Console.WriteLine(Strings.IsPalindrome("hello"));     // False
Console.WriteLine(Strings.IsPalindrome("a"));         // True
Console.WriteLine(Strings.IsPalindrome(""));          // True
Console.WriteLine(Strings.IsPalindrome("ab"));        // False
Console.WriteLine(Strings.IsPalindrome("aa"));        // True
Console.WriteLine(Strings.IsPalindrome("aba"));       // True

var map = new SimpleHashMap<string, int>();
map.Put("apple", 1);
map.Put("banana", 2);
map.Put("apple", 99);

Console.WriteLine(map.Get("apple"));        // 99
Console.WriteLine(map.Get("banana"));       // 2
Console.WriteLine(map.Contains("apple"));   // True
Console.WriteLine(map.Contains("cherry"));  // False
Console.WriteLine(map.Count);               // 2

map.Remove("apple");
Console.WriteLine(map.Contains("apple"));   // False
Console.WriteLine(map.Count);               // 1

try
{
    map.Get("xyz");
}
catch (KeyNotFoundException e)
{
    Console.WriteLine($"KeyNotFound: {e.Message}");
}

var list = new SinglyLinkedList<int>();
foreach (var value in new[] { 1, 2, 3, 4 })
    list.AddLast(value);

list.Reverse();
Console.WriteLine(Format(list.AsEnumerable()));   // [4, 3, 2, 1]

var root = new TreeNode<int>(
    1,
    new TreeNode<int>(2, new TreeNode<int>(4), new TreeNode<int>(5)),
    new TreeNode<int>(3, right: new TreeNode<int>(6)));

foreach (var level in Tree.LevelOrder(root))
    Console.WriteLine(Format(level));

Console.WriteLine(Strings.ValidParentheses("({[]})"));            // True
Console.WriteLine(Strings.ValidParentheses("(]"));                // False
Console.WriteLine(Strings.ValidParenthesesIgnoringOtherChars("a(b[c]d)e"));  // True

int[][] grid =
[
    [0, 0, 0],
    [0, -1, 0],
    [0, 0, 0],
];

foreach (var (r, c, dist) in Traversals.BfsGrid(grid, 0, 0))
    Console.Write($"({r},{c},{dist}) ");

Console.WriteLine();

static string Format<T>(IEnumerable<T> items) => $"[{string.Join(", ", items)}]";
