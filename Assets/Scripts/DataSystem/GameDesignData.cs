using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    public class GameDesignData
    {
        public static CharacterData GetCharacterDataById(string id)
        {
            return Resources.Load<CharacterData>("ScriptableObjects/CharacterData/" + id);
        }
    }

}