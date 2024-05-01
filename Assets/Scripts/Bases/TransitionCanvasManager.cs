using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace Cameras
{
    public class TransitionCanvasManager : MonoSingleton<TransitionCanvasManager>
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}