# Dsa

Ćwiczenia DSA przepisane z `dsa.py` na .NET 10.

## Struktura

- `src/Dsa` — biblioteka: grafy, przechodzenia, lista jednokierunkowa, wyszukiwanie binarne, stringi, hash mapa, drzewo
- `src/Dsa.Runner` — konsola odtwarzająca demo z oryginalnego skryptu
- `tests/Dsa.Tests` — xUnit

## Uruchomienie

```
dotnet run --project src/Dsa.Runner
dotnet test
```

## Różnice względem `dsa.py`

- `WeightedGraph.add_edge` dla grafu nieskierowanego dodawał krawędź `v -> v`; w C# jest `v -> u`.
- `levelOrder` miał `for _ in len(queue)` i brak `return`; w C# iteracja po rozmiarze poziomu i zwrot wyniku.
- `SimpleHashMap` rzuca `KeyNotFoundException` zamiast `KeyError`.
