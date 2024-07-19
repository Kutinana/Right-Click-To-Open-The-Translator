using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using Translator;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObjectManager : MonoSingleton<InteractableObjectManager>
{
    public GameObject Prefab;
    public static InteractableObject Current;

    private void Awake()
    {
        TypeEventSystem.Global.Register<OnItemUseEvent>(e => {
            if (Current != null) return;
            Current = Instantiate(Prefab, transform).GetComponent<InteractableObject>().Initialize(e.Data);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    public static void DropAndExit()
    {
        Current.DropAndExit();
        Current = null;
    }
}
