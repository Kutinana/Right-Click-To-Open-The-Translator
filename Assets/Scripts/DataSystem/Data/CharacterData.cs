using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/Character Data", order = 0)]
    public class CharacterData : ScriptableObject, IHaveId
    {
        [SerializeField] private string id;
        public string Id => id;
        
        public Sprite Sprite;
        [Multiline] public string Description;

        public List<PuzzleDataBase> RelatedPuzzles;
    }
}