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

    public static bool IsHolding(string id)
    {
        return Current != null && Current.Data.Id == id;
    }

    public static void Exit()
    {
        Destroy(Current.gameObject);
        Current = null;
    }

    public static void DropAndExit()
    {
        AudioKit.PlaySound("Fall-Zero", volumeScale: 0.2f);
        Current.DropAndExit();
        Current = null;
    }

    public static void DropAndGet(string id)
    {
        Current.DropAndGet(GameDesignData.GetObtainableObjectDataById(id));
        Current = null;
    }
}
