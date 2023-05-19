using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class MeshGeneratorBase
{

    public CellGeometrySettings cellGeometrySettings;

    public bool cellIsSelected = false;
    public bool cellIsMouseover = false;
    

    protected List<Vector3> _vertices;
    protected List<int> _triangles;
    protected List<Color> _colors;
    protected Dictionary<Vector3, int> _vertexLookup = new Dictionary<Vector3, int>();

    public void SetGeoLists(List<Vector3> vertices, List<int> triangles, List<Color> colors)
    {
        _vertices = vertices;
        _triangles = triangles;
        _colors = colors;
    }

    public virtual void MeshifyCell(HexCell cell)
    {
        
    }

    protected void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
    {
        var i1 = AddVertex(v1, color);
        var i2 = AddVertex(v2, color);
        var i3 = AddVertex(v3, color);
        _triangles.Add(i1);
        _triangles.Add(i2);
        _triangles.Add(i3);
    }

    protected void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color)
    {
        var i1 = AddVertex(v1, color);
        var i2 = AddVertex(v2, color);
        var i3 = AddVertex(v3, color);
        var i4 = AddVertex(v4, color);
        _triangles.Add(i1);
        _triangles.Add(i3);
        _triangles.Add(i2);
        _triangles.Add(i2);
        _triangles.Add(i3);
        _triangles.Add(i4);
    }

    protected int AddVertex(Vector3 vertex, Color color)
    {
        /*
        if (_vertexLookup.ContainsKey(vertex))
        {
            return _vertexLookup[vertex];
        }
        */
        var index = _vertices.Count;
        _vertices.Add(vertex);
        _colors.Add(color);
        _vertexLookup[vertex] = index;
        return index;
    }
}
