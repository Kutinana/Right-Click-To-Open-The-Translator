using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "MissionData", menuName = "Scriptable Objects/Mission Data", order = 0)]
    public class MissionData : ScriptableObject
    {
        public string Id;
        public string Name;
        [Multiline] public string Description;

        public List<string> Prerequisite;
    }
}