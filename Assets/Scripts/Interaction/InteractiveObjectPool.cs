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

    private static int MAX_CAPACITY = 64;
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
            //e.puzzle.Id
            Instance.activeObject.gameObject.GetComponent<InteractivePuzzle>()?.OnT.Invoke();
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
        //Debug.Log(id + " " + interactiveObject.name + " is triggered");
        interactiveObject.TriggerEvent();
        //EventTrigger(delegate () { interactiveObject.TriggerEvent(); });
    }
    public static void ObjectEndTrigger()
    {
        Instance.activeObject.EndTrigger();
    }
    public static void EventTrigger(params System.Action[] actions)
    {
        foreach (System.Action action in actions) AddEvent(action);
        EventList.Invoke();
        foreach (System.Action action in actions) RemoveEvent(action);
    }
    public static void EventTrigger(System.Action action)
    {
        AddEvent(action);
        EventList.Invoke();
        RemoveEvent(action);
    }
    private static void AddEvent(System.Action action)
    {
        EventList += delegate
        {
            action();
        };
    }
    private static void RemoveEvent(System.Action action)
    {
        EventList -= delegate
        {
            action();
        };
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