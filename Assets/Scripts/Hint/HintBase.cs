using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;

namespace Hint
{
    public abstract class HintBase : MonoBehaviour
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
    }

}