using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UI;
using UnityEngine;

namespace Puzzle
{
    public abstract class PuzzleBase : MonoBehaviour
    {
        public virtual void OnEnter()
        {
            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.Unlock(ids);
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnExit()
        {

        }

        public virtual void OnComplete()
        {

        }
    }

}