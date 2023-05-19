using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexplaneGenerator
{
    private HexCell[] _cells;
    public HexCell[] GetCells() {return _cells;}

    private Vector2Int _chunkDimensions;
    private TerrainChunkMesh _chunk;

    public void Generate(Vector2Int chunkDimensions, TerrainChunkMesh chunk)
    {
        _chunkDimensions = chunkDimensions;
        _chunk = chunk;
        GenerateCells();
    }

    private void GenerateCells()
    {
        _cells = new HexCell[_chunkDimensions.x * _chunkDimensions.y];
        var i = 0;
        for (int x = 0; x < _chunkDimensions.x; x++)
        {
            for (int z = 0; z < _chunkDimensions.y; z++)
            {
                var cell = GenerateCell(x, z, i);
                _cells[i] = cell;
                i++;
            }
        }
    }

    private HexCell GenerateCell(int x, int z, int i)
    {
        var cell = new HexCell();
        cell.chunk = _chunk;
        cell.index = i;
        cell.LocalCoordinates = new HexCoordinates(x,z);

        return cell;
    }
}
