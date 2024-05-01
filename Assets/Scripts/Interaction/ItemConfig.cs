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

    };
}


