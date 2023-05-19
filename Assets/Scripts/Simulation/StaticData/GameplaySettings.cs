using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu()]
public class GameplaySettings : ScriptableObject
{
    [Range(1, 30)]
    public float manaSpawnTimeMin = 1f;
    [Range(1, 30)]
    public float manaSpawnTimeMax = 1f;
}