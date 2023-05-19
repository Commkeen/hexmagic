using System.Collections.Generic;
using UnityEngine;

public class PolyCell
{
    // Simple grid info
    public int index;
    public int face;
    private List<PolyCell> _neighbors;

    // Simple geo info
    protected Vector3 _center;
    protected List<Vector3> _corners;
    
    // Additional generated properties
    public Color color;
    public float elevation;

    // Sim tile
    private TileState _tile;
    public TileState Tile {get {return _tile;} set {_tile = value; _tile.cell = this;}}

    // Owner
    public TerrainChunkMesh chunk;
    

    public PolyCell()
    {

    }

    public virtual int VertexCount()
    {
        return _corners.Count + 1;
    }

    public virtual Vector3 GetUpVector()
    {
        return _center.normalized;
    }

    public virtual Vector3 GetCenter()
    {
        return _center;
    }

    public void SetChunkDirty()
    {
        chunk.dirty = true;
    }

    public virtual (Vector3, Vector3) GetCornersForSide(int side)
    {
        if (side == _corners.Count) {side = 0;}
        if (side == -1) {side = _corners.Count-1;}
        Debug.Assert(side >= 0 && side < _corners.Count);
        if (side == _corners.Count-1)
        {
            return (_corners[_corners.Count-1], _corners[0]);
        }
        return (_corners[side], _corners[side+1]);
    }

    // Returns the vector from an inner corner to the closest point on the outer side.
    public virtual Vector3 GetBridge(int side, float outerCellSize)
    {
        (Vector3 a, Vector3 b) = GetCornersForSide(side);
        var cornerAFromCenter = a - GetCenter();
        var cornerBFromCenter = b - GetCenter();
        var midpoint = (cornerAFromCenter + cornerBFromCenter) * 0.5f;
        var midpointScaled = midpoint * outerCellSize;
        return midpointScaled;
    }

    public virtual PolyCell GetNeighborForSegment(int side)
    {
        (Vector3 cornerA, Vector3 cornerB) = GetCornersForSide(side);
        PolyCell result = null;
        foreach (var c in _neighbors)
        {
            if (c._corners.Contains(cornerA) && c._corners.Contains(cornerB))
            {
                result = c;
                break;
            }
        }
        Debug.Assert(result != null, "GetNeighborForSegment couldn't find a neighbor!");
        return result;
    }
}
