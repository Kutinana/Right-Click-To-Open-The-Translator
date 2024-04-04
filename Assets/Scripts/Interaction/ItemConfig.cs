using System;
using UnityEngine;

public enum ItemType
{
    DOOR,
    PUZZLE,
    OTHERS
}

[Serializable]
public class ItemConfig
{
    public int ID;
    public String TriggeredSpriteName;
    public String itemType;
}

[Serializable]
public class DoorConfig
{
    public int ID;
}