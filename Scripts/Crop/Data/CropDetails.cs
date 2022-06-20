using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CropDetails 
{
    public int seedItemID;
    [Header("Days need to grow")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("Different stage item Prefab")]
    public GameObject[] growthPrefabs;

    [Header("Different stage image")]
    public Sprite[]growthSprites;
    [Header("Seasons allowed to be plant")]
    public Season[] seasons;

    [Space]
    [Header("Harvest Tool")]
    public int[] harvestToolItemID;
    [Header("Required Action")]
    public int[] requireActionCount;
    [Header("transfer new Item ID")]
    public int transferItemID;

    [Space]
    [Header("Harvest item information")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("Days to Regrow")]
    public int daysToRegrow;
    public int regrowTime;

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;
    //TODO: Effect voice

    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;

        }
        return false;
    }

    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if(harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }

}
