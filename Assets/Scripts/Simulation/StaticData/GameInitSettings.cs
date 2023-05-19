using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu()]
public class GameInitSettings : ScriptableObject
{
    [Range(2, 10)]
    public int exploredRadius = 3;
    [Range(2, 10)]
    public int miasmaClearedRadius = 2;
}