using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    DOOR = 0,
    PUZZLE = 1,
    NPC = 2,
    Hint = 3
}

[Serializable]
public class ItemConfig
{
    public int ID;
    public String OutlinedSpritePath;
    public String UnoutlinedSpritePath;
    public ItemType itemType;
    public String target_string;
}

public static class DoorConfig
{
    public static Dictionary<int, String> nextSceneName = new Dictionary<int, string>()
    {
        {8, "BeginPlace" },
        {12, "First" },
        {18, "First" },
        {32, "Classroom" },
        {33, "Church" },
        {34, "InMarket" },
        {35, "InMarket" },
        {36, "InMarket" },
        {37, "OutCenter" },
        {38, "InMarket" },
        {39, "InEnergy" },
        {40, "BeginPlace" },
        {41, "InMarket" },
        {42, "InCenter" },
        {43, "OutCenter" }
    };
}


