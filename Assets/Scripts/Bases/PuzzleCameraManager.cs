using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace Cameras
{
    public class PuzzleCameraManager : MonoSingleton<PuzzleCameraManager>
    {
        public static Camera Camera => Instance.GetComponent<Camera>();
        void Awake() {}
    }

}