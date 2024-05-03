using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/NPCSentencesData", fileName = "NPC")]
public class NPCSentencesData: ScriptableObject
{
    public int ID;
    public int statement_length;
    public Sprite[] SentencesData;
    public SentenceType[] SentenceTypes;
    public bool needToAnswer;
    public GameObject[] AnswerChoices;
}

public enum SentenceType
{
    NPC_Statement,
    Player_Answer,
    NPC_Answer
}