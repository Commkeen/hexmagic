using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class CloudMeshGenerator : MeshGeneratorBase
{

    public static Color miasmaColor = new Color(1,1,1,0.75f);

    public override void MeshifyCell(HexCell cell)
    {
        if (!cell.Tile.miasma && cell.Tile.explored) {return;}
        Profiler.BeginSample("Meshify cell");
        _vertexLookup.Clear();
        for (int i = 0; i < cell.VertexCount(); i++)
        {
            MeshifyCellSegment(cell, i);
        }
        Profiler.EndSample();
    }

    private float GetElevation(HexCell cell)
    {
        if (cell.Tile.explored)
        {
            return 1 + cellGeometrySettings.elevationStep*(cell.elevation+0.2f);
        }
        else
        {
            return 1 + cellGeometrySettings.elevationStep*5;
        }
    }

    private void MeshifyCellSegment(HexCell cell, int side)
    {
        var innerSize = cellGeometrySettings.innerCellSize;

        var color = miasmaColor;
        if (!cell.Tile.explored)
        {
            color.r = 0;
            color.a = 1;
        }

        (Vector3 cornerA, Vector3 cornerB) = cell.GetCornersForSide(side);
        var center = cell.GetCenter();

        var innerA = Vector3.Lerp(center, cornerA, innerSize);
        var innerB = Vector3.Lerp(center, cornerB, innerSize);

        var innerElevationFactor = GetElevation(cell);

        var centerElevated = center + Vector3.up*innerElevationFactor;
        var innerAElevated = innerA + Vector3.up*innerElevationFactor;
        var innerBElevated = innerB + Vector3.up*innerElevationFactor;

        AddTriangle(centerElevated, innerAElevated, innerBElevated, color);

        // Inner cell is done, now let's draw the outer edges
        var neighbor = (HexCell)cell.GetNeighborForSegment(side);
        var prevNeighbor = (HexCell)cell.GetNeighborForSegment(side-1);
        var nextNeighbor = (HexCell)cell.GetNeighborForSegment(side+1);

        var bridge = cell.GetBridge(side, cellGeometrySettings.outerCellSize);
        var bridgeA = innerA + bridge;
        var bridgeB = innerB + bridge;

        var outerElevationFactor = innerElevationFactor;

        var bridgeAElevated = bridgeA + Vector3.up*outerElevationFactor;
        var bridgeBElevated = bridgeB + Vector3.up*outerElevationFactor;
        var cornerAElevated = cornerA + Vector3.up*outerElevationFactor;
        var cornerBElevated = cornerB + Vector3.up*outerElevationFactor;

        var outerColor = color;
        // If our neighbor doesn't have clouds, fade these clouds out
        if (neighbor != null && !neighbor.Tile.miasma)
        {
            outerColor.a = 0.25f;
        }

        // AddQuad but with outerColor
        //AddQuad(innerAElevated, innerBElevated, bridgeAElevated, bridgeBElevated, color);
        var i1 = AddVertex(innerAElevated, color);
        var i2 = AddVertex(innerBElevated, color);
        var i3 = AddVertex(bridgeAElevated, outerColor);
        var i4 = AddVertex(bridgeBElevated, outerColor);
        _triangles.Add(i1);
        _triangles.Add(i3);
        _triangles.Add(i2);
        _triangles.Add(i2);
        _triangles.Add(i3);
        _triangles.Add(i4);

        // AddTriangle but with outerColor
        //AddTriangle(innerAElevated, cornerAElevated, bridgeAElevated, color);
        i1 = AddVertex(innerAElevated, color);
        i2 = AddVertex(cornerAElevated, outerColor);
        i3 = AddVertex(bridgeAElevated, outerColor);
        _triangles.Add(i1);
        _triangles.Add(i2);
        _triangles.Add(i3);
        
        //AddTriangle(innerBElevated, bridgeBElevated, cornerBElevated, color);
        i1 = AddVertex(innerBElevated, color);
        i2 = AddVertex(bridgeBElevated, outerColor);
        i3 = AddVertex(cornerBElevated, outerColor);
        _triangles.Add(i1);
        _triangles.Add(i2);
        _triangles.Add(i3);
        

        // If we have a neighbor at a different elevation on our first 3 sides,
        // draw a quad from our outer edge to its outer edge
        if (side >= 3) {return;}
        if (neighbor != null && neighbor.Tile.miasma && neighbor.elevation != cell.elevation)
        {
            (Vector3 nCornerA, Vector3 nCornerB) = cell.GetCornersForSide(side);
            var nElevationFactor = GetElevation(neighbor);
            var nCornerAElevated = nCornerA + Vector3.up*nElevationFactor;
            var nCornerBElevated = nCornerB + Vector3.up*nElevationFactor;
            AddQuad(cornerAElevated, cornerBElevated, nCornerAElevated, nCornerBElevated, color);
        }
    }
}
