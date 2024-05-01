using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Cameras
{
    public class EventSystemManager : MonoSingleton<EventSystemManager>
    {
        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }

}