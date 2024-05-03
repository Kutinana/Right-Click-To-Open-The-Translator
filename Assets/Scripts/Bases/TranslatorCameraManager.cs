using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.Rendering.PostProcessing;
using XPostProcessing;

namespace Cameras
{
    public class TranslatorCameraManager : MonoSingleton<TranslatorCameraManager>
    {
        public static Camera Camera => Instance.GetComponent<Camera>();
        void Awake() {}
    }

}