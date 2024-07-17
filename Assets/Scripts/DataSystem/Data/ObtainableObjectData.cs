using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "ObtainableObjectData", menuName = "Scriptable Objects/Obtainable Object Data", order = 0)]
    public class ObtainableObjectData : ScriptableObject, IHaveId
    {
        public string id;
        public string Name;
        public Sprite Sprite;
        [Multiline] public string Description;
        [Multiline] public string SubDescription;

        public int MaxAmount = 0;

        public string Id => id;
    }
}