using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/Character Data", order = 0)]
    public class CharacterData : ScriptableObject
    {
        public string Id;
        public Sprite Sprite;
        [Multiline] public string Description;

        public List<PuzzleDataBase> RelatedPuzzles;
    }
}