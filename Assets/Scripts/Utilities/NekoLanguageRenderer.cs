using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;

public class NekoLanguageRenderer : MonoBehaviour
{
    public static List<Sprite> RenderToSprite(string origin, string separator = " ")
    {
        List<Sprite> result = new List<Sprite>();
        foreach (var c in origin.Split(separator))
        {
            result.Add(GameDesignData.GetCharacterDataById(c).Sprite);
        }
        return result;
    }

    public static List<CharacterData> RenderToCharacterData(string origin, string separator = " ")
    {
        List<CharacterData> result = new List<CharacterData>();
        foreach (var c in origin.Split(separator))
        {
            result.Add(GameDesignData.GetCharacterDataById(c));
        }
        return result;
    }
}
