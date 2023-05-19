using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A GameObject that lives on the map and is the child of a terrain chunk
[RequireComponent(typeof(MapPosition))]
public class MapObject : MonoBehaviour
{
    public MapObjectCollection mapObjects;

    public TerrainChunkMesh LocalChunk {get; set;}
    public HexCell Cell {get; set;}

    // Accessors
    public HexCoordinates WorldCoordinates {get {return GetComponent<MapPosition>().WorldCoordinates;}}

    void OnDisable()
    {
        mapObjects.Remove(this);
    }

    public void SetCoordinates(HexCoordinates coords)
    {
        var pos = GetComponent<MapPosition>();
        var onMap = pos.OnMap;
        pos.SetCoordinates(coords);
        if (!onMap)
        {
            mapObjects.Add(this);
            SendMessage("OnActivateEffect", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
