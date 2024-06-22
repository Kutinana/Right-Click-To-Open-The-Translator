using System.Collections;
using System.Collections.Generic;
using Cameras;
using UnityEngine;

namespace Cameras
{
    [RequireComponent(typeof(Camera))]
    public class DefaultCamera : MonoBehaviour
    {
        private void Awake()
        {
            BaseCameraManager.AddToCameraStack(GetComponent<Camera>());
        }

        private void OnDestroy()
        {
            BaseCameraManager.RemoveFromCameraStack(GetComponent<Camera>());
        }
    }
}
