using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Localization;
using UnityEngine;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "ObtainableObjectData", menuName = "Scriptable Objects/Obtainable Object Data", order = 0)]
    public class ObtainableObjectData : ScriptableObject, IHaveId
    {
        public string id;
        public SerializableDictionary<Language, string> Name;
        public Sprite Sprite;
        public SerializableDictionary<Language, string> Description;
        public SerializableDictionary<Language, string> SubDescription;

        public int MaxAmount = 0;

        public string Id => id;
    }
}