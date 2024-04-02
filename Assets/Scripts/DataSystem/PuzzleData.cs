using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "PuzzleData", menuName = "Scriptable Objects/Puzzle Data", order = 0)]
    public class PuzzleData : ScriptableObject
    {
        public string Id;
        public GameObject PuzzlePrefab;
        public Sprite Thumbnail;
        public string Description;
    }
}