using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UI;
using UnityEngine;

namespace Puzzle
{
    public abstract class PuzzleBase : MonoBehaviour
    {
        public string Id;
        public virtual void OnEnter()
        {
            
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