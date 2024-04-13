using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    DOOR,
    PUZZLE,
    NPC
}

[Serializable]
public class ItemConfig
{
    public int ID;
    public String OutlinedSpritePath;
    public String UnoutlinedSpritePath;
    public String itemType;
}

public static class DoorConfig
{
    public static Dictionary<int, String> nextSceneName = new Dictionary<int, string>()
    {

    };
}


