using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : PolyCell
{
    public HexCoordinates WorldCoordinates {get; set;}
    public HexCoordinates LocalCoordinates {get; set;}
    public RectTransform UIRect {get; set;}
    
    
    // Local to the chunk origin
    public Vector3 SurfaceCenter {get; set;}
    public Transform Transform {get; set;}

    public float waveOffset = 0;

    private HexCell[] _neighbors = new HexCell[6];

    public HexCell()
    {

    }

    public void SetNeighbor(HexCell neighbor, HexDirection dir)
    {
        _neighbors[(int)dir] = neighbor;
    }

    public HexCell GetNeighbor(HexDirection dir)
    {
        return _neighbors[(int)dir];
    }

    public override int VertexCount()
    {
        return 6;
    }

    public override Vector3 GetUpVector()
    {
        return Vector3.up;
    }

    public override Vector3 GetCenter()
    {
        return LocalCoordinates.ToWorldCoordinates();
    }

    public override (Vector3, Vector3) GetCornersForSide(int side)
    {
        if (side == -1) {side = 5;}
        if (side == 6) {side = 0;}
        Debug.Assert(side >= 0 && side <= 5);
        if (side == 5)
        {
            return (HexMetrics.corners[5] + GetCenter(), HexMetrics.corners[0] + GetCenter());
        }
        return (HexMetrics.corners[side]+GetCenter(), HexMetrics.corners[side+1]+GetCenter());
    }

    public override PolyCell GetNeighborForSegment(int side)
    {
        if (side == -1) {side = 5;}
        if (side == 6) {side = 0;}
        Debug.Assert(side >= 0 && side <= 5);
        var dir = (HexDirection)side;
        var neighbor = GetNeighbor(dir);
        return neighbor;
    }






}
