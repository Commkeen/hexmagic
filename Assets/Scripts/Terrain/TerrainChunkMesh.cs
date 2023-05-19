using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainChunkMesh : MonoBehaviour
{
    public TerrainMesh terrain;
    public Vector2Int offset;
    public bool init = false;
    public bool dirty = false;

    public HexCell[] cells;

    private Canvas _canvas;

    private Transform _localObjects;
    private MapChunkLayerMesh _terrainLayer;
    private MapChunkLayerMesh _miasmaLayer;

    void Update()
    {
        foreach (var cell in cells)
        {
            cell.waveOffset = terrain.mapWaves.GetWaveTotal(cell.WorldCoordinates)*10;
        }
    }

    public void CreateChunk(TerrainMesh terrain)
    {
        this.terrain = terrain;
        
        _localObjects = new GameObject("Local Objects").transform;
        _localObjects.transform.SetParent(transform);

        _terrainLayer = new GameObject("Terrain Mesh").AddComponent<MapChunkLayerMesh>();
        _terrainLayer.transform.SetParent(transform);
        _terrainLayer.Create(this, terrain.terrainMaterial, true);
        _terrainLayer.meshGenerator = new FlatHexCellMeshGenerator();

        _miasmaLayer = new GameObject("Miasma Mesh").AddComponent<MapChunkLayerMesh>();
        _miasmaLayer.transform.SetParent(transform);
        _miasmaLayer.Create(this, terrain.miasmaMaterial, false);
        _miasmaLayer.meshGenerator = new CloudMeshGenerator();

        _canvas = GetComponentInChildren<Canvas>();
        if (_canvas == null)
        {
            var canvasObj = new GameObject("Chunk Canvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.Rotate(90,0,0);
            _canvas = canvasObj.AddComponent<Canvas>();
            var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;
        }
    }

    public void CreateCells(Vector2Int chunkDimensions)
    {
        var generator = new HexplaneGenerator();
        generator.Generate(chunkDimensions, this);
        cells = generator.GetCells();

        foreach (var cell in cells)
        {
            var label = new GameObject("Cell Label");
            var txt = label.AddComponent<UnityEngine.UI.Text>();
            txt.text = "X\nX\nX";
            txt.font = terrain.uiFont;
            txt.fontSize = 3;
            txt.alignment = TextAnchor.MiddleCenter;
            
            cell.UIRect = txt.rectTransform;
            cell.UIRect.SetParent(_canvas.transform, false);
            cell.UIRect.anchoredPosition = new Vector2(cell.GetCenter().x, cell.GetCenter().z);
            cell.UIRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 12);
            cell.UIRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 12);

            cell.Transform = new GameObject($"Cell {cell.LocalCoordinates}").transform;
            cell.Transform.SetParent(_localObjects);
            cell.Transform.position = cell.GetCenter();
        }
    }

    public void SetChunkOffset(Vector2Int offset)
    {
        this.offset = offset;
        init = true;
        gameObject.name = $"Chunk {offset.x}, {offset.y}";
        var chunkPos = new HexCoordinates(offset.x*terrain.ChunkDimensions.x, offset.y*terrain.ChunkDimensions.y);
        transform.position = chunkPos.ToWorldCoordinates();

        foreach (var cell in cells)
        {
            cell.WorldCoordinates = new HexCoordinates(cell.LocalCoordinates.X + chunkPos.X, cell.LocalCoordinates.Z + chunkPos.Z);
            cell.Tile = terrain.tiles.Get(cell.WorldCoordinates);
            //cell.UIRect.GetComponent<UnityEngine.UI.Text>().text = cell.WorldCoordinates.ToStringOnSeparateLines();
        }

        foreach (var mapObj in _localObjects.GetComponentsInChildren<MapObject>())
        {
            GameObject.Destroy(mapObj.gameObject); // TODO: This is bad actually, we need to persist these...
        }

        dirty = true;
    }

    public void RebuildMesh()
    {
        //Debug.Log($"Rebuild chunk {gameObject.name}");
        SetupCellNeighbors();
        EvaluateFilters();
        _terrainLayer.RebuildMesh();
        _miasmaLayer.RebuildMesh();

        _canvas.enabled = terrain.showCellLabels;
        _canvas.GetComponent<UnityEngine.UI.CanvasScaler>().dynamicPixelsPerUnit = 10;

        dirty = false;
    }

    public void AddMapObjectToChunk(MapObject obj)
    {
        obj.LocalChunk = this;
        var parentCell = GetCellAtWorldCoords(obj.WorldCoordinates);
        obj.Cell = parentCell;
        obj.transform.SetParent(parentCell.Transform, false);
    }

    public MapObject GetMapObject(HexCoordinates worldCoords)
    {
        foreach (var obj in _localObjects.GetComponentsInChildren<MapObject>())
        {
            if (obj.WorldCoordinates == worldCoords) {return obj;}
        }
        return null;
    }

    private void SetupCellNeighbors()
    {
        UnityEngine.Profiling.Profiler.BeginSample($"Setup cell neighbors for chunk {gameObject.name}");
        foreach (var cell in cells)
        {
            for (var i = 0; i < 6; i++)
            {
                var dir = (HexDirection)i;
                var coord = cell.WorldCoordinates.GetNeighbor(dir);
                var neighbor = terrain.GetCellAtCoords(coord);
                cell.SetNeighbor(neighbor, dir);
                if (neighbor == null) {continue;}
                if (neighbor.GetNeighbor(dir.Opposite()) == null)
                {
                    neighbor.SetNeighbor(cell, dir.Opposite());
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void EvaluateFilters()
    {
        var colorFilter = new ColorFromTileStateFilter(terrain.colorSettings);
        foreach (var cell in cells)
        {
            colorFilter.Evaluate(cell);
        }
    }

    public Vector3 GetLocalCenterSurfaceForCell(HexCoordinates coords)
    {
        var cell = GetCellAtWorldCoords(coords);
        Debug.Assert(cell != null);
        return cell.SurfaceCenter;
    }

    public HexCell GetCellAtWorldCoords(HexCoordinates coords)
    {
        foreach (var cell in cells)
        {
            if (cell.WorldCoordinates == coords) {return cell;}
        }
        return null;
    }

    public HexCell GetCellAtWorldPosition(Vector3 pos)
    {
        HexCell cell = null;
        var sqDist = Mathf.Infinity;
        for (int i = 0; i < cells.Length; i++)
        {
            var c = cells[i];
            var d = (pos - (c.GetCenter()+transform.position)).sqrMagnitude;
            if (sqDist > d)
            {
                cell = c;
                sqDist = d;
            }
        }
        return cell;
    }

    public Vector3 GetMousePositionOnMesh()
    {
        return _terrainLayer.GetMousePositionOnMesh();
    }
}
