using UnityEngine;

public class ElevationFilter
{
    ElevationSettings settings;

    public ElevationFilter(ElevationSettings settings)
    {
        this.settings = settings;
    }

    public void Evaluate(HexCell cell)
    {
        if (settings == null) {return;}

        if (settings.minElevation > settings.maxElevation) {settings.maxElevation = settings.minElevation;}

        var noisePosition = new Vector2(cell.WorldCoordinates.X, cell.WorldCoordinates.Z) * settings.roughness;
        noisePosition += (Vector2)settings.center;

        float noiseResult = Mathf.PerlinNoise(noisePosition.x, noisePosition.y)*.5f + .5f;

        if (noiseResult < settings.waterCoverage) {noiseResult = 0;}
        noiseResult = Mathf.InverseLerp(settings.waterCoverage, 1f, noiseResult);

        int result = Mathf.FloorToInt(Mathf.Lerp(settings.minElevation, settings.maxElevation, noiseResult));
        result = Mathf.Clamp(result, settings.minElevation, settings.maxElevation);

        cell.elevation = result;
    }
}