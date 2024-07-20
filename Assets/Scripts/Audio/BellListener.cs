using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Configuration;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;

public class BellListener : MonoBehaviour
{
    public float vol = 1;
    private void Start()
    {
        TypeEventSystem.Global.Register<DayLightController.NewCircleEvent>(e => AudioKit.PlaySound("ChurchBell", volumeScale: vol)).UnRegisterWhenCurrentSceneUnloaded();
    }
}
