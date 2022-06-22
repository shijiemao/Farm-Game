using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData")]

public class MapData_SO : ScriptableObject
{

    [SceneName] public string sceneName;
    [header("Map Information")]
    public int gridWith;
    public int gridHeight;
    [Header("Original Point")]
    public int originX;
    public int originY;
    public List<TileProperty> tileProperties;
}
