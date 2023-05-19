using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TileFilter : ScriptableObject
{
    public ElevationSettings elevationSettings;
    public ColorSettings terrainTypeSettings;

    public void Evaluate(TileState tile)
    {
        var elevation = EvaluateElevation(tile);
        var terrainType = EvaluateTerrainType(elevation);
        ApplyTerrainType(terrainType, tile);
    }

    private int EvaluateElevation(TileState tile)
    {
        if (elevationSettings.minElevation > elevationSettings.maxElevation) {elevationSettings.maxElevation = elevationSettings.minElevation;}

        var noisePosition = new Vector2(tile.WorldCoordinates.X, tile.WorldCoordinates.Z) * elevationSettings.roughness;
        noisePosition += (Vector2)elevationSettings.center;

        float noiseResult = GetNoise(noisePosition)*.5f + .5f;

        if (noiseResult < elevationSettings.waterCoverage) {noiseResult = 0;}
        noiseResult = Mathf.InverseLerp(elevationSettings.waterCoverage, 1f, noiseResult);

        int result = Mathf.FloorToInt(Mathf.Lerp(elevationSettings.minElevation, elevationSettings.maxElevation, noiseResult));
        result = Mathf.Clamp(result, elevationSettings.minElevation, elevationSettings.maxElevation);

        return result;
    }

    private int EvaluateTerrainType(int elevation)
    {
        if (elevation <= terrainTypeSettings.waterHeight)
        {
            return 0;
        }
        else if (elevation <= terrainTypeSettings.beachHeight)
        {
            return 1;
        }
        else if (elevation <= terrainTypeSettings.lowlandHeight)
        {
            return 2;
        }
        else if (elevation <= terrainTypeSettings.highlandHeigh)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    private void ApplyTerrainType(int terrainType, TileState tile)
    {
        switch (terrainType)
        {
            case 0:
                tile.Elevation = 0;
                tile.Life = 0;
                break;
            case 1:
                tile.Elevation = 1;
                tile.Life = 0;
                break;
            case 2:
                tile.Elevation = 1;
                tile.Life = 1;
                break;
            case 3:
                tile.Elevation = 1;
                tile.Life = 2;
                break;
            case 4:
                tile.Elevation = 2;
                tile.Life = 0;
                break;
        }
    }

    private float GetNoise(Vector2 pos)
    {
        return Mathf.PerlinNoise(pos.x, pos.y);
    }
}
