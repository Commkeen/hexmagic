using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapWaves : MonoBehaviour
{
    public List<Wave> waves = new List<Wave>();

    // Update is called once per frame
    void Update()
    {
        var wavesToDelete = new List<Wave>();
        foreach (var wave in waves)
        {
            wave.currentTime += Time.deltaTime;
            if (wave.currentTime*wave.speed > wave.distance*2)
            {
                wavesToDelete.Add(wave);
            }
        }

        foreach (var wave in wavesToDelete)
        {
            waves.Remove(wave);
        }
    }

    public void AddWave(HexCoordinates coords)
    {
        var wave = new Wave();
        wave.origin = coords;
        waves.Add(wave);
    }

    public float GetWaveTotal(HexCoordinates coords)
    {
        var result = 0f;
        foreach (var wave in waves)
        {
            var distance = coords.DistanceTo(wave.origin);
            if (wave.distance < distance) {continue;}
            var v = wave.GetValue(distance);
            result += v;
        }
        return result;
    }

}
