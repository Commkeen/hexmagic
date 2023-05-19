using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimWorld : MonoBehaviour
{
    public GameInitSettings gameInitSettings;
    public GameplaySettings gameplaySettings;
    public TerrainMesh map;

    public TileCollection tiles;
    public HexCoordinateSet exploredTiles;
    public HexCoordinateSet miasmaTiles;
    public MapObjectCollection mapObjects;
    public MapObjectLibrary mapObjectLibrary;
    public TileFilter mapGenTileFilter;

    private float _manaDropSpawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _manaDropSpawnTimer -= Time.fixedDeltaTime;
        if (_manaDropSpawnTimer <= 0)
        {
            var coords = FindTileToSpawnManaDrop();
            if (coords != null)
            {
                //SpawnManaDrop(coords.Value);
            }
            _manaDropSpawnTimer = Random.Range(gameplaySettings.manaSpawnTimeMin, gameplaySettings.manaSpawnTimeMax);
        }
    }

    public void Init()
    {
        var center = new HexCoordinates(0,0);
        //InitExploredHexes();
        //var genCoords = center.GetAllInRadius(8);
        //foreach (var c in genCoords)
        //{
        //    tiles.Get(c);
        //}
        var tower = GameObject.Instantiate<MapObject>(mapObjectLibrary.tower);
        tower.SetCoordinates(new HexCoordinates(0,0));
        _manaDropSpawnTimer = Random.Range(gameplaySettings.manaSpawnTimeMin, gameplaySettings.manaSpawnTimeMax);
    }

    private void SpawnManaDrop(HexCoordinates coords)
    {
        var tile = tiles.Get(coords);
        tile.hasManaDrop = true;
        var manaDrop = GameObject.Instantiate<MapObject>(mapObjectLibrary.manaDrop);
        manaDrop.SetCoordinates(coords);
        //map.AddManaDrop(coords);
        //Debug.Log($"Spawned mana at tile {tile.WorldCoordinates}");
    }

    private HexCoordinates? FindTileToSpawnManaDrop()
    {
        HexCoordinates? result = null;
        var attempts = 0;
        var maxAttempts = 50;
        while (attempts < maxAttempts)
        {
            attempts++;
            var c = exploredTiles.GetRandom();
            var tile = tiles.Get(c);
            if (tile.miasma || tile.hasManaDrop) {continue;}
            result = c;
            break;
        }

        return result;
    }


}
