using Newtonsoft.Json;
using Puzzle;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectPool
{
    private static InteractiveObjectPool _instance;
    public static InteractiveObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InteractiveObjectPool();
                Init();
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private static int MAX_CAPACITY = 128;
    private static List<InteractiveObject>[] ObjectPool = new List<InteractiveObject>[MAX_CAPACITY];
    private static System.Action EventList;

    public List<ItemConfig> itemConfigs = new List<ItemConfig>();
    public InteractiveObject activeObject = null;
    private static void Init()
    {
        var r_itemConfig = Resources.Load<TextAsset>("Config/ItemConfig").text;
        _instance.itemConfigs = JsonConvert.DeserializeObject<List<ItemConfig>>(r_itemConfig);

        TypeEventSystem.Global.Register<OnPuzzleSolvedEvent>(e =>
        {
            if (Instance.activeObject == null) return;
            if (Instance.activeObject.gameObject.GetComponent<InteractivePuzzle>() == null) return;

            Instance.activeObject.gameObject.GetComponent<InteractivePuzzle>().OnT?.Invoke();
        });
    }
    public static int getMaxSize() => ObjectPool.Length;
    public static void LoadObject(InteractiveObject interactiveObject)
    {
        int id = interactiveObject.ID;
        if (ObjectPool[id] == null)
        {
            ObjectPool[id] = new List<InteractiveObject>();
        }
        int pos = SearchObject(interactiveObject);
        if (pos < 0) ObjectPool[id].Add(interactiveObject);
        //else Debug.Log("Redundantly load: " + id + " " + interactiveObject.name);
    }
    public static void RemoveObject(InteractiveObject interactiveObject)
    {
        int id = interactiveObject.ID;
        if (ObjectPool[id] == null)
        {
            Debug.Log("Could not find id: " + id);
            return;
        }
        int pos = SearchObject(interactiveObject);
        if (pos >= 0) ObjectPool[id][pos] = null;
        //else Debug.Log("Could not find object in pool: " + id + " " + interactiveObject.name);
    }
    public static int SearchObject(InteractiveObject interactiveObject)
    {
        int id = interactiveObject.ID;
        for (int i = 0; i < ObjectPool[id].Count; i++)
        {
            if (ObjectPool[id][i] == interactiveObject)
            {
                return i;
            }
        }
        return -1;
    }
    public static void ObjectTriggered(InteractiveObject interactiveObject)
    {
        int id = interactiveObject.ID;
        interactiveObject.TriggerEvent();
    }
    public static void ObjectEndTrigger()
    {
        Instance.activeObject.EndTrigger();
    }
    public static void SetActiveObject(InteractiveObject interactiveObject)
    {
        if (_instance.activeObject != null && _instance.activeObject != interactiveObject) _instance.activeObject.Deactivate();
        if (_instance.activeObject != interactiveObject)
        {
            _instance.activeObject = interactiveObject;
            if (_instance.activeObject != null) _instance.activeObject.Activate();
        }
    }
    public static void RefreshActiveObject(InteractiveObject interactiveObject)
    {
        if (interactiveObject == _instance.activeObject)
        {
            _instance.activeObject.Activate();
        }
    }
}