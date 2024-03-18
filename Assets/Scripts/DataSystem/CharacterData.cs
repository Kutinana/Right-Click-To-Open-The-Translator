using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 0)]
    public class CharacterData : ScriptableObject
    {
        public string Id;
        public Sprite Sprite;
        public string Description;
    }
}