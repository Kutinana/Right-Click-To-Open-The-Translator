using QFramework;
using UnityEngine;

public class MapCanvasManager : MonoSingleton<MapCanvasManager>
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}