using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behaviour to hold a gameobject's position on the hex map.
public class MapPosition : MonoBehaviour
{
    public HexCoordinates WorldCoordinates {get;private set;}
    public bool OnMap {get; private set;} = false;

    void OnDisable()
    {
        //TODO: Remove from coordinate set
    }

    public void SetCoordinates(HexCoordinates coords)
    {
        OnMap = true;
        WorldCoordinates = coords;

        //TODO: Update coordinate set
    }
}
