using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class FlatHexCellMeshGenerator : MeshGeneratorBase
{
    public override void MeshifyCell(HexCell cell)
    {
        if (!cell.Tile.explored) {return;}
        Profiler.BeginSample("Meshify cell");
        _vertexLookup.Clear();
        for (int i = 0; i < cell.VertexCount(); i++)
        {
            MeshifyCellSegment(cell, i);
        }
        Profiler.EndSample();
    }

    private void MeshifyCellSegment(HexCell cell, int side)
    {

        var innerSize = cellGeometrySettings.innerCellSize;

        var color = cell.color;
        //if (cellIsSelected) {color = Color.red;}
        //else if (cellIsMouseover) {color = Color.yellow;}
        //if (side == 0) {color = Color.Lerp(color, Color.red, 0.4f);}
        //if (side == 1) {color = Color.Lerp(color, Color.red, 0.2f);}

        (Vector3 cornerA, Vector3 cornerB) = cell.GetCornersForSide(side);
        var center = cell.GetCenter();

        var innerA = Vector3.Lerp(center, cornerA, innerSize);
        var innerB = Vector3.Lerp(center, cornerB, innerSize);

        center += Vector3.up*cell.waveOffset;
        cornerA += Vector3.up*cell.waveOffset;
        cornerB += Vector3.up*cell.waveOffset;
        innerA += Vector3.up*cell.waveOffset;
        innerB += Vector3.up*cell.waveOffset;



        var innerElevationFactor = 1 + cellGeometrySettings.elevationStep * cell.elevation;

        var centerElevated = center + Vector3.up*innerElevationFactor;
        var innerAElevated = innerA + Vector3.up*innerElevationFactor;
        var innerBElevated = innerB + Vector3.up*innerElevationFactor;

        cell.SurfaceCenter = centerElevated;
        cell.Transform.localPosition = centerElevated;

        AddTriangle(centerElevated, innerAElevated, innerBElevated, color);

        // Inner cell is done, now let's draw the outer edges
        var bridge = cell.GetBridge(side, cellGeometrySettings.outerCellSize);
        var bridgeA = innerA + bridge;
        var bridgeB = innerB + bridge;

        AddQuad(innerAElevated, innerBElevated, bridgeA, bridgeB, color);
        AddTriangle(innerAElevated, cornerA, bridgeA, color);
        AddTriangle(innerBElevated, bridgeB, cornerB, color);
    }
}
