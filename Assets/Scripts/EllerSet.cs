using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EllerSet
{
    private Dictionary<int, List<Vector2Int>> sets = new Dictionary<int, List<Vector2Int>>();
    private Dictionary<Vector2Int, int> cellToSet = new Dictionary<Vector2Int, int>();
    private int nextSetId = 1;

    public bool AreInSameSet(Vector2Int cell1, Vector2Int cell2)
    {
        return cellToSet.ContainsKey(cell1) && cellToSet.ContainsKey(cell2) && cellToSet[cell1] == cellToSet[cell2];
    }

    public void AddCell(Vector2Int cell)
    {
        if (!cellToSet.ContainsKey(cell))
        {
            int setId = nextSetId++;
            sets[setId] = new List<Vector2Int> { cell };
            cellToSet[cell] = setId;
        }
    }

    public void MergeSets(Vector2Int cell1, Vector2Int cell2)
    {
        if (AreInSameSet(cell1, cell2)) return;

        int set1 = cellToSet[cell1];
        int set2 = cellToSet[cell2];

        foreach (var cell in sets[set2])
        {
            sets[set1].Add(cell);
            cellToSet[cell] = set1;
        }

        sets.Remove(set2);
    }

    public bool IsInSet(Vector2Int cell) 
    { 
        return cellToSet.ContainsKey(cell); 
    }

    public IEnumerable<int> GetSetIds()
    {
        return sets.Keys.ToList();
    }

    public IEnumerable<Vector2Int> GetCellsInSet(int setId)
    {
        if (sets.ContainsKey(setId))
        {
            return sets[setId];
        }
        return new List<Vector2Int>();
    }


    public void PrepareNextRow(List<Vector2Int> cellsToKeep)
    {
        var newSets = new Dictionary<int, List<Vector2Int>>();
        var newCellToSet = new Dictionary<Vector2Int, int>();

        //crear nuevos conjuntos solo con las celdas que se desean mantener
        foreach (var cell in cellsToKeep)
        {
            if (cellToSet.TryGetValue(cell, out int setId))
            {
                if (!newSets.ContainsKey(setId))
                {
                    newSets[setId] = new List<Vector2Int>();
                }
                newSets[setId].Add(cell);
                newCellToSet[cell] = setId;
            }
            else
            {
                //si la celda no estaba en un conjunto anterior, se asigna uno nuevo.
                int newSetId = nextSetId++;
                newSets[newSetId] = new List<Vector2Int> { cell };
                newCellToSet[cell] = newSetId;
            }
        }

        //se reemplazan los conjuntos y mapeos de celdas antiguos con los nuevos.
        sets = newSets;
        cellToSet = newCellToSet;
    }

}
