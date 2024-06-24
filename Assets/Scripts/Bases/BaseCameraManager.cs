using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.Rendering.Universal;

namespace Cameras
{
    public class BaseCameraManager : MonoSingleton<BaseCameraManager>
    {
        public static Camera Camera;
        private void Awake()
        {
            Camera = GetComponent<Camera>();
        }

        public static void AddToCameraStack(Camera camera)
        {
            var cameraData = Camera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Insert(0, camera);
        }

        public static void RemoveFromCameraStack(Camera camera)
        {
            var cameraData = Camera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Remove(camera);
        }
    }

}