using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileState
{
    // Unchanging properties
    public HexCoordinates WorldCoordinates {get; private set;}

    // Changing properties
    public bool init = false;
    private int _elevation = 1;
    public int Elevation {get {return _elevation;} set {SetElevation(value);}} // 0=ocean, 1=land, 2=mountain
    private int _life = 1;
    public int Life {get {return _life;} set {SetLife(value);}}
    public bool miasma = true;
    public bool explored = false;
    public bool hasManaDrop = false;

    // Render cell
    public PolyCell cell;

    public TileState(HexCoordinates coords)
    {
        WorldCoordinates = coords;
    }

    private void SetElevation(int value)
    {
        _elevation = value;
        cell?.SetChunkDirty();
    }

    private void SetLife(int value)
    {
        _life = value;
        cell?.SetChunkDirty();
    }
}
