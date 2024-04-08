using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/NPCSentencesData", fileName = "NPC")]
public class NPCSentencesData: ScriptableObject
{
    public int ID;
    public Sprite[] SentencesData;
}