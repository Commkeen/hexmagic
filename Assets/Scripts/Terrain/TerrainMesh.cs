using System.Collections;
using System.Collections.Generic;
using Diag = System.Diagnostics;
using System.Linq;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{

    [Range(10, 1000)]
    public uint maxRebuildTime = 250;

    [Range(0, 5)]
    public int chunkPregenRadius = 1;

    [Range(4, 20)]
    public int chunkWidth = 10;
    [Range(4, 20)]
    public int chunkHeight = 10;

    [Range(2, 10)]
    public int exploredRadius = 3;
    [Range(2, 10)]
    public int miasmaClearedRadius = 2;

    public bool showCellLabels = false;

    public string cameraPosHex = "";
    public string cameraChunk = "";
    public string mouseOverCoords = "";

    private Vector2Int _currentChunkDim = Vector2Int.zero;
    public Vector2Int ChunkDimensions {get {return _currentChunkDim;}}

    public Material terrainMaterial;
    public Material miasmaMaterial;
    public Font uiFont;

    public MapObjectLibrary mapObjectLibrary;
    public CellGeometrySettings cellGeometrySettings;
    public ColorSettings colorSettings;
    public ElevationSettings elevationSettings;

    public CoordsEvent tileClicked;

    public HexCoordinates? mouseOverCell = null;
    public HexCoordinates? selectedCell = null;

    public TileCollection tiles;

    private Vector2Int _currentCenterChunk = Vector2Int.zero;
    private HexCoordinates _cameraCenterHex;

    public MapWaves mapWaves;

    private CameraTarget _cameraTarget;
    private TerrainChunkMesh[] _chunks;

    void Start()
    {
        mapWaves = GetComponent<MapWaves>();
        _cameraTarget = GameObject.FindFirstObjectByType<CameraTarget>();
        GenerateMesh();
    }

    void Update()
    {
        UpdateMouse();
        UpdateCenterChunk();

        if (!RebuildDirtyChunks(maxRebuildTime))
        {
            //UpdateCellFilters();
            DirtyChunksWithActivity();
        }
    }

    private void UpdateMouse()
    {
        mouseOverCell = null;
        if (Input.GetMouseButtonDown(1))
        {
            selectedCell = null;
        }
        foreach (var chunk in _chunks)
        {
            if (chunk == null) {continue;}
            if (!chunk.enabled) {continue;}
            var mousePos = chunk.GetMousePositionOnMesh();
            if (mousePos != Vector3.zero)
            {
                var cell = chunk.GetCellAtWorldPosition(mousePos);
                if (cell != null)
                {
                    mouseOverCell = cell.WorldCoordinates;
                    mouseOverCoords = cell.WorldCoordinates.ToString();
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedCell = cell.WorldCoordinates;
                        //SimWorld.Instance.ClickTile(cell.WorldCoordinates);
                        tileClicked?.Invoke(cell.WorldCoordinates);
                        //Debug.Log($"Selected cell {cell.WorldCoordinates}");
                        //Debug.Log($"Neighbor at SE is {cell.GetNeighbor(HexDirection.SE).WorldCoordinates}");
                        //Debug.Log($"Neighbor at NW is {cell.GetNeighbor(HexDirection.SE).WorldCoordinates}");
                    }
                }
            }
        }
    }

    public void UpdateCenterChunk()
    {
        _cameraCenterHex = HexCoordinates.FromPosition(_cameraTarget.transform.position);
        cameraPosHex = _cameraCenterHex.ToString();
        var cameraCenterChunkCoords = GetChunkOffsetForPosition(_cameraTarget.transform.position);
        cameraChunk = cameraCenterChunkCoords.ToString();
        if (cameraCenterChunkCoords != _currentCenterChunk)
        {
            //Debug.Log($"New center chunk is {cameraCenterChunkCoords}");
            _currentCenterChunk = cameraCenterChunkCoords;
            InitNewChunksUsingOldChunks(_currentCenterChunk);
            //SetupCellNeighbors();
            //RunCellFilters();
            RebuildMesh();
        }
    }

    public void OnInspectorChange()
    {
        if (chunkWidth != _currentChunkDim.x ||
            chunkHeight != _currentChunkDim.y)
        {
            GenerateMesh();
            return;
        }

        UpdateCellFilters();
    }

    public void UpdateCellFilters()
    {
        //RunCellFilters();
        RebuildMesh();
    }

    public void GenerateMesh()
    {
        //SimWorld.Instance.Init();

        _currentChunkDim = new Vector2Int(chunkWidth, chunkHeight);

        if (cellGeometrySettings == null)
        {
            cellGeometrySettings = ScriptableObject.CreateInstance<CellGeometrySettings>();
        }

        if (_cameraTarget)
        {
            _currentCenterChunk = GetChunkOffsetForPosition(_cameraTarget.transform.position);
        }

        DestroyAllChunks();
        CreateChunkList();
        InitAllChunks(_currentCenterChunk);
        //SetupCellNeighbors();
        //RunCellFilters();
        RebuildMesh();
    }

    // Create TerrainChunkMesh.
    // Should be the only place we init new gameobjects unless we change chunk dimensions
    // since we can reuse these chunks as we unload and load new sections.
    private void CreateChunkList()
    {
        Debug.Log("Instantiating chunks.");
        _chunks = new TerrainChunkMesh[(chunkPregenRadius*2+1)*(chunkPregenRadius*2+1)];
        for (int i = 0; i < _chunks.Length; i++)
        {
            var chunk = new GameObject($"Chunk Noninit").AddComponent<TerrainChunkMesh>();
            _chunks[i] = chunk;
            chunk.transform.SetParent(transform, false);
            chunk.CreateChunk(this);
            chunk.CreateCells(_currentChunkDim);
        }
    }

    private void DestroyAllChunks()
    {
        if (_chunks != null)
        {
            foreach (var chunk in _chunks)
            {
                if (chunk == null) {continue;}
                if (chunk?.gameObject == null) {continue;}
                chunk.transform.SetParent(null);
                GameObject.DestroyImmediate(chunk.gameObject);
            }
        }
        foreach (var chunk in transform.GetComponentsInChildren<TerrainChunkMesh>())
        {
            if (chunk == null) {continue;}
            GameObject.DestroyImmediate(chunk.gameObject);
        }

        _chunks = new TerrainChunkMesh[(chunkPregenRadius*2+1)*(chunkPregenRadius*2+1)];
    }

    // Overwrite all chunks with what we currently want loaded.
    private void InitAllChunks(Vector2Int center)
    {
        Debug.Assert(_chunks.Length > 0);
        var reuseQueue = new Queue<TerrainChunkMesh>();
        foreach (var chunk in _chunks)
        {
            reuseQueue.Enqueue(chunk);
        }
        InitChunks(center, reuseQueue);
    }

    // Find chunks out of our radius and overwrite them with new chunks.
    private void InitNewChunksUsingOldChunks(Vector2Int center)
    {
        UnityEngine.Profiling.Profiler.BeginSample("Recycle chunks");
        Debug.Assert(_chunks.Length > 0);
        var minX = center.x-chunkPregenRadius;
        var maxX = center.x+chunkPregenRadius;
        var minZ = center.y-chunkPregenRadius;
        var maxZ = center.y+chunkPregenRadius;
        var reuseQueue = new Queue<TerrainChunkMesh>();

        foreach (var chunk in _chunks)
        {
            if (chunk.offset.x < minX || chunk.offset.x > maxX ||
                chunk.offset.y < minZ || chunk.offset.y > maxZ)
            {
                reuseQueue.Enqueue(chunk);
            }
        }

        InitChunks(center, reuseQueue);
        UnityEngine.Profiling.Profiler.EndSample();
    }

    // Leave loaded chunks in radius alone and overwrite chunksToReuse with new chunks.
    // Assumes that chunksToReuse contains exactly the right number!
    private void InitChunks(Vector2Int center, Queue<TerrainChunkMesh> chunksToReuse)
    {
        //Debug.Log($"InitChunks at {center} with reuse queue of {chunksToReuse.Count}");
        Debug.Assert(_chunks.Length > 0);
        var minX = center.x-chunkPregenRadius;
        var maxX = center.x+chunkPregenRadius;
        var minZ = center.y-chunkPregenRadius;
        var maxZ = center.y+chunkPregenRadius;

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                var chunk = GetChunkWithOffset(new Vector2Int(x,z));
                if (chunk == null)
                {
                    var oldChunk = chunksToReuse.Dequeue();
                    //Debug.Log($"Init chunk for {x},{z}, reuse chunk {oldChunk.gameObject.name}");
                    oldChunk.SetChunkOffset(new Vector2Int(x,z));
                }
                else
                {
                    //Debug.Log($"Found chunk for {x},{z}, name {chunk.gameObject.name}");
                }
            }
        }

        Debug.Assert(chunksToReuse.Count == 0);
    }

    private void SetupCellNeighbors()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Setup cell neighbors");
        foreach (var chunk in _chunks)
        {
            if (!chunk.enabled) {continue;}
            foreach (var cell in chunk.cells)
            {
                for (var i = 0; i < 6; i++)
                {
                    var dir = (HexDirection)i;
                    var coord = cell.WorldCoordinates.GetNeighbor(dir);
                    var neighbor = GetCellAtCoords(coord);
                    cell.SetNeighbor(neighbor, dir);
                    if (neighbor == null) {continue;}
                    if (neighbor.GetNeighbor(dir.Opposite()) == null)
                    {
                        neighbor.SetNeighbor(cell, dir.Opposite());
                    }
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void RunCellFilters()
    {
        var colorFilter = new ColorFromTileStateFilter(colorSettings);

        foreach (var chunk in _chunks)
        {
            foreach (var cell in chunk.cells)
            {
                colorFilter.Evaluate(cell);
            }
        }
    }

    private void RebuildMesh()
    {
        if (Application.isPlaying)
        {
            DirtyAllChunks();
        }
        else
        {
            RebuildAllChunks();
        }
    }

    private void RebuildAllChunks()
    {
        for(int i = 0; i < _chunks.Length; i++)
        {
            var chunk = _chunks[i];
            if (chunk.enabled)
            {
                chunk.RebuildMesh();
            }
        }
    }

    private void DirtyAllChunks()
    {
        for(int i = 0; i < _chunks.Length; i++)
        {
            var chunk = _chunks[i];
            chunk.dirty = true;
        }
    }

    private void DirtyChunksWithActivity()
    {
        if (mouseOverCell != null)
        {
            GetChunkAtWorldCoords(mouseOverCell.Value).dirty = true;
        }
        if (selectedCell != mouseOverCell && selectedCell != null)
        {
            GetChunkAtWorldCoords(selectedCell.Value).dirty = true;
        }
        GetChunkWithOffset(_currentCenterChunk).dirty = true;

        var centerChunks = GetChunksInRadius(_cameraCenterHex, 7);
        foreach (var chunk in centerChunks)
        {
            chunk.dirty = true;
        }

        foreach (var wave in mapWaves.waves)
        {
            var affectedChunks = GetChunksInRadius(wave.origin, Mathf.CeilToInt(wave.distance));
            foreach (var c in affectedChunks)
            {
                c.dirty = true;
            }
        }
    }

    private bool RebuildDirtyChunks(uint maxTime)
    {
        var stopwatch = new Diag.Stopwatch();
        var dirtyChunkFound = false;
        stopwatch.Start();

        // Always rebuild certain priority chunks
        

        for(int i = 0; i < _chunks.Length; i++)
        {
            var chunk = _chunks[i];
            if (chunk == null) {continue;}
            if (chunk.dirty && chunk.enabled)
            {
                dirtyChunkFound = true;
                chunk.RebuildMesh();
            }
            if (stopwatch.ElapsedMilliseconds > maxTime) {break;}
        }
        stopwatch.Stop();
        return dirtyChunkFound;
    }

    public void AddManaDrop(HexCoordinates worldCoords)
    {
        var prefab = mapObjectLibrary.manaDrop;
        var obj = GameObject.Instantiate<MapObject>(prefab);

        obj.GetComponent<MapPosition>().SetCoordinates(worldCoords);
        var chunk = GetChunkAtWorldCoords(worldCoords);
        chunk.AddMapObjectToChunk(obj);
    }

    public void RemoveManaDrop(HexCoordinates worldCoords)
    {
        var chunk = GetChunkAtWorldCoords(worldCoords);
        var manaDrop = chunk.GetMapObject(worldCoords);
        Debug.Assert(manaDrop != null);
        if (manaDrop != null)
        {
            GameObject.Destroy(manaDrop.gameObject);
        }
    }

    public HexCell GetCellAtCoords(HexCoordinates coords)
    {
        var chunk = GetChunkAtWorldCoords(coords);
        if (chunk == null) {return null;}
        return chunk.GetCellAtWorldCoords(coords);
    }

    public void PlaceMapObject(MapObject obj)
    {
        var worldCoords = obj.WorldCoordinates;
        var chunk = GetChunkAtWorldCoords(worldCoords);
        //Debug.Log($"Add {obj.gameObject.name} at coords {worldCoords} to chunk {chunk}");
        chunk.AddMapObjectToChunk(obj);
    }

    private TerrainChunkMesh GetChunkWithOffset(Vector2Int offset)
    {
        foreach (var chunk in _chunks)
        {
            if (chunk.init && chunk.offset == offset) {return chunk;}
        }
        return null;
    }

    private TerrainChunkMesh GetChunkAtWorldCoords(HexCoordinates coords)
    {
        foreach (var chunk in _chunks)
        {
            var chunkCoords = chunk.offset;
            var chunkMinX = chunkCoords.x*_currentChunkDim.x;
            var chunkMaxX = (chunkCoords.x+1)*_currentChunkDim.x;
            var chunkMinZ = chunkCoords.y*_currentChunkDim.y;
            var chunkMaxZ = (chunkCoords.y+1)*_currentChunkDim.y;
            if (coords.X >= chunkMinX && coords.X < chunkMaxX &&
                coords.Z >= chunkMinZ && coords.Z < chunkMaxZ)
            {
                return chunk;
            }
        }
        return null;
    }

    private List<TerrainChunkMesh> GetChunksInRadius(HexCoordinates coords, int radius)
    {
        var results = new List<TerrainChunkMesh>();
        foreach (var chunk in _chunks)
        {
            var chunkCoords = chunk.offset;
            var chunkMinX = chunkCoords.x*_currentChunkDim.x;
            var chunkMaxX = (chunkCoords.x+1)*_currentChunkDim.x;
            var chunkMinZ = chunkCoords.y*_currentChunkDim.y;
            var chunkMaxZ = (chunkCoords.y+1)*_currentChunkDim.y;
            var radiusMinX = coords.X-radius;
            var radiusMaxX = coords.X+radius;
            var radiusMinZ = coords.Z-radius;
            var radiusMaxZ = coords.Z+radius;
            var overlap = true;
            if (radiusMinX > chunkMaxX) {overlap = false;}
            if (radiusMaxX < chunkMinX) {overlap = false;}
            if (radiusMinZ > chunkMaxZ) {overlap = false;}
            if (radiusMaxZ < chunkMinZ) {overlap = false;}
            if (overlap) {results.Add(chunk);}
        }
        return results;
    }

    private Vector2Int GetChunkOffsetForPosition(Vector3 position)
    {
        var hexCoords = HexCoordinates.FromPosition(position);
        var offsetX = hexCoords.X / _currentChunkDim.x;
        if (hexCoords.X < 0) {offsetX -= 1;}
        var offsetZ = hexCoords.Z / _currentChunkDim.y;
        if (hexCoords.Z < 0) {offsetZ -= 1;}
        return new Vector2Int(offsetX, offsetZ);
    }
}
