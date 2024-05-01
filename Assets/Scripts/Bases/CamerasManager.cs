using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace Cameras
{
    public class CamerasManager : MonoSingleton<CamerasManager>
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}