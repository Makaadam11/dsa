from collections import defaultdict, deque
class WeightedGraph:
    def __init__(self, directed = False):
        self.adj = defaultdict(list)
        self.directed = directed

    def add_edge(self, u, v, weight = 1):
        self.adj[u].append({v: weight})
        if self.directed == True:
            self.adj[v].append({v: weight})

class Graph:
    def __init__(self, directed = False):
        self.adj = defaultdict(list)
        self.directed = directed

    def add_edge(self, u, v):
        self.adj[u].append(v)
        if not self.directed:
            self.adj[v].append(u)

def dfs_recursive(graph, node, visited = None, result = None):
    if visited is None:
        visited = set()
    if result is None:
        result = []

    visited.add(node)
    result.append(node)

    for neighbour in graph[node]:
        if neighbour not in visited:
            dfs_recursive(graph, neighbour, visited, result)

    return result


def dfs_iterative(graph, node):
    result = []
    stack = [node]
    visited = set()

    while stack:
        node = stack.pop()
        if node in visited:
            continue

        visited.add(node)
        result.append(node)
        for neighbour in reversed(graph[node]):
            if neighbour not in visited:
                stack.append(neighbour)

    return result


def bfs(graph, node):
    queue = deque([node])
    visited = {node}
    result = []

    while queue:
        node = queue.popleft()
        result.append(node)

        for neighbour in graph[node]:
            if neighbour not in visited:
                visited.add(neighbour)
                queue.append(neighbour)

    return result

def bfs_grid(grid, start_r, start_c):
    rows, cols = len(grid), len(grid[0])
    directions = [(0,1),(0,-1),(1,0),(-1,0)]
    visited = {(start_r, start_c)}
    queue = deque([(start_r, start_c, 0)])
    result = []

    while queue:
        r, c, dist = queue.popleft()
        result.append((r, c, dist))
        for (dr, dc) in directions:
            nr, nc = r + dr, c + dc
            if (0 <= nr < rows and 0 <= nc < cols) and (nr, nc) not in visited and grid[nr][nc] != -1: # edge/wall depents on constraints
                visited.add((nr, nc))
                queue.append((nr, nc, dist + 1))

    return result

class ListNode():
    def __init__(self, value = 0, next = None):
        self.value = value
        self.next = next

class LinkedList():
    def __init__(self):
        self.head = None
        self.size = 0

    def reverse(self):
        current = self.head
        prev = None

        while current:
            next_node = current.next
            current.next = prev
            prev = current
            current = next_node

        self.head = prev

def valid_parentheses(string: str) -> bool:
    mapping = {')' : '(', '}' : '{', ']': '['}
    stack = []

    for char in string:
        if char in mapping:
            if not stack or stack[-1] != mapping[char]:
                return False
            stack.pop()
        else:
            stack.append(char)

    return len(stack) == 0

def valid_parentheses_extra(string: str) -> bool:
    mapping = {')' : '(', '}' : '{', ']': '['}
    openings = set(mapping.values())
    stack = []

    for char in string:
        if char in openings:
            stack.append(char)
        elif char in mapping:
            if not stack or stack[-1] != mapping[char]:
                return False
            stack.pop()

    return len(stack) == 0

def binary_search(arr, target):
    left, right = 0, len(arr) - 1

    while left <= right:
        mid = (left + right) // 2
        if arr[mid] == target:
            return mid
        if arr[mid] < target:
            left = mid + 1
        else:
            right = mid - 1
    return - 1

def binary_search_bounds(arr, target):
    left, right = 0, len(arr)

    while left < right:
        mid = (left + right) // 2
        if arr[mid] < target:
            left = mid + 1
        else:
            right = mid
    return left


def is_palindrome(string: str) -> bool:
    left, right = 0, len(string) - 1

    while left < right:
        if string[left] != string[right]:
            return False
        left += 1
        right -= 1
    return True

        

g = Graph(True)
g.add_edge(1, 2)
g.add_edge(1, 3)
g.add_edge(2, 4)
g.add_edge(2, 5)
g.add_edge(3, 6)

print(dfs_recursive(g.adj, 1))
print(dfs_iterative(g.adj, 1))
print(bfs(g.adj, 1))

arr = [1, 3, 5, 7, 9]

# Klasyczny
print(binary_search(arr, 5))     # 2
print(binary_search(arr, 4))     # -1
print(binary_search(arr, 1))     # 0
print(binary_search(arr, 9))     # 4

# Lower_bound
print(binary_search_bounds(arr, 5))     # 2
print(binary_search_bounds(arr, 4))     # 2 (insertion point)
print(binary_search_bounds(arr, 0))     # 0
print(binary_search_bounds(arr, 10))    # 5

# Z duplikatami
arr2 = [1, 3, 3, 3, 5, 7]
print(binary_search_bounds(arr2, 3))    # 1 (PIERWSZE wystąpienie)


print(is_palindrome("racecar"))    # True
print(is_palindrome("hello"))      # False
print(is_palindrome("a"))          # True (jeden znak)
print(is_palindrome(""))           # True (pusty string)
print(is_palindrome("ab"))         # False
print(is_palindrome("aa"))         # True
print(is_palindrome("aba"))        # True (parzyste left=0/right=2, potem left=1=right=1, koniec)

class SimpleHashMap:
    def __init__(self, capacity=16):
        self._capacity = capacity
        self._buckets = [[] for _ in range(capacity)]
        self._size = 0
    
    def _hash(self, key):
        return hash(key) % self._capacity
    
    def put(self, key, value):
        index = self._hash(key)
        bucket = self._buckets[index]
        
        for i, (k, _) in enumerate(bucket):
            if k == key:
                bucket[i] = (key, value)
                return
        
        bucket.append((key,value))
        self._size += 1
        
    def get(self, key):
        index = self._hash(key)
        bucket = self._buckets[index]
        
        for (k, v) in bucket:
            if k == key:
                return v
        
        raise KeyError(key)
    
    def remove(self, key):
        index = self._hash(key)
        bucket = self._buckets[index]
        
        for i, (k, _) in enumerate(bucket):
            if k == key:
                del bucket[i]
                self._size -= 1
                return
        
        raise KeyError(key)
    
    def contains(self, key) -> bool:
        index = self._hash(key)
        return any(k == key for k, _ in self._buckets[index])

hm = SimpleHashMap()

hm.put("apple", 1)
hm.put("banana", 2)
hm.put("apple", 99)              # update

print(hm.get("apple"))            # 99
print(hm.get("banana"))           # 2
print(hm.contains("apple"))       # True
print(hm.contains("cherry"))      # False
print(hm._size)                   # 2 (apple liczy się raz)

hm.remove("apple")
print(hm.contains("apple"))       # False
print(hm._size)                   # 1

# Edge case: KeyError
try:
    hm.get("xyz")
except KeyError as e:
    print(f"KeyError: {e}")       # KeyError: 'xyz'
    

class TreeNode:
    def __init__(self, value, left, right):
        self.value = value
        self.left = left
        self.right = right
        
def levelOrder(root: TreeNode):
    if not root:
        return []
    queue = deque([root])
    result = []

    while queue:
        level = []
        for _ in len(queue):
            node = queue.popleft()
            level.append(node.value)
            
            if node.left:
                queue.append(node.left)
            if node.right:
                queue.append(node.right)
                
        result.append(level)