using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "HintData", menuName = "Scriptable Objects/Hint Data", order = 0)]
    public class HintData : ScriptableObject
    {
        public string Id;
        public GameObject HintPrefab;
        public Sprite Thumbnail;
        public string Description;
    }
}