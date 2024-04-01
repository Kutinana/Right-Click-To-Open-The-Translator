using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "SentenceData", menuName = "Scriptable Objects/Sentence Data", order = 0)]
    public class SentenceData : ScriptableObject
    {
        public string Id;
        public List<string> Sentence;
        public string CorrectMeaning;
        public string Description;
    }
}