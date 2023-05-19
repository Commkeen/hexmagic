using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapChunkLayerMesh : MonoBehaviour
{
    public TerrainChunkMesh chunk;
    public MeshGeneratorBase meshGenerator;

    private bool _assignedMeshToCollider = false;
    private Mesh _mesh;
    private MeshCollider _collider;
    private List<Vector3> _vertices;
    private List<int> _triangles;
    private List<Color> _colors;
    private Dictionary<Vector3, int> _vertexLookup;

    public void Create(TerrainChunkMesh chunk, Material material, bool hasCollider)
    {
        this.chunk = chunk;
        if (_mesh == null) {_mesh = new Mesh();}
        GetComponent<MeshFilter>().mesh = _mesh;
        GetComponent<MeshRenderer>().material = material;
        _mesh.MarkDynamic();
        if (hasCollider)
        {
            _collider = GetComponent<MeshCollider>();
            if (_collider == null) {_collider = gameObject.AddComponent<MeshCollider>();}
            _collider.convex = true;
        }
        _mesh.name = "Chunk Layer Mesh";
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _colors = new List<Color>();
        _vertexLookup = new Dictionary<Vector3, int>();
    }

    public void RebuildMesh()
    {
        _mesh.Clear();
        _vertices.Clear();
        _triangles.Clear();
        _colors.Clear();
        _vertexLookup.Clear();

        Debug.Assert(chunk.cells.Length > 0);
        MeshifyCells(chunk.cells);

        _mesh.vertices = _vertices.ToArray();
        _mesh.colors = _colors.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateNormals();
        _mesh.MarkModified();

        if (_collider != null && !_assignedMeshToCollider)
        {
            _collider.sharedMesh = _mesh;
            _collider.enabled = true;
            _assignedMeshToCollider = true;
        }
        if (_collider != null)
        {
            _collider.enabled = false;
            if (_triangles.Count > 0)
            {
                _collider.enabled = true;
            }
        }
    }

    private void MeshifyCells(HexCell[] cells)
    {
        Debug.Assert(meshGenerator != null);
        Profiler.BeginSample($"Meshify Cells for {chunk.gameObject.name} {gameObject.name}");
        meshGenerator.cellGeometrySettings = chunk.terrain.cellGeometrySettings;
        meshGenerator.SetGeoLists(_vertices, _triangles, _colors);

        for (int i = 0; i < cells.Length; i++)
        {
            meshGenerator.cellIsMouseover = cells[i].WorldCoordinates == chunk.terrain.mouseOverCell;
            meshGenerator.cellIsSelected = cells[i].WorldCoordinates == chunk.terrain.selectedCell;
            
            meshGenerator.MeshifyCell(cells[i]);
            var uirect = cells[i].UIRect;
            var uirectPos = uirect.anchoredPosition3D;
            uirectPos.z = (cells[i].elevation+1) * chunk.terrain.cellGeometrySettings.elevationStep * -1;
            uirect.anchoredPosition3D = uirectPos;
        }
        Profiler.EndSample();
    }

    public Vector3 GetMousePositionOnMesh()
    {
        if (_collider == null) {return Vector3.zero;}
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider != _collider) {return Vector3.zero;}
            return hit.point;
        }
        return Vector3.zero;
    }
}
