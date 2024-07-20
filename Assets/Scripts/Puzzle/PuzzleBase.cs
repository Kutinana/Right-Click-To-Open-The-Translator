using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
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
            AudioKit.PlaySound("InteractClick", volumeScale: AudioMng.Instance.effectVolume * 0.8f);
        }

        public virtual void OnSolved()
        {

        }
    }

}