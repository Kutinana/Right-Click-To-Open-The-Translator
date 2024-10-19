using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Configuration;
using QFramework;
using UnityEngine;
using static DayLightController;

public class BellListener : MonoBehaviour
{
    public float vol = 1;
    private void Awake()
    {
        TypeEventSystem.Global.Register<NewCircleEvent>(e => AudioKit.PlaySound("ChurchBell", volume: vol)).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
}
