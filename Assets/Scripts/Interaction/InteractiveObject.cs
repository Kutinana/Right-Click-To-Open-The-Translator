using DataSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;

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
        else Debug.Log("Redundantly load: " + id + " " + interactiveObject.name);
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
        else Debug.Log("Could not find object in pool: " + id + " " + interactiveObject.name);
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
        Debug.Log(id + " " + interactiveObject.name + " is triggered");
        interactiveObject.TriggerEvent();
        //EventTrigger(delegate () { interactiveObject.TriggerEvent(); });
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
}
public interface Interactive
{
    public int ID { get; }
    public bool activable { get; }
    public void OnTrigger();
    public void EndTrigger();
    public void Activate();
    public void Deactivate();
}

public class InteractiveObject : MonoBehaviour, Interactive
{
    public int identity_number;
    public ItemConfig itemConfig;
    private SpriteRenderer spriteRenderer;
    private bool _activable;
    public int count = -1;
    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public int ID 
    {
        get { return identity_number; }
    }
    public bool activable
    {
        get { return _activable; }
    }
    public virtual void LoadConfig()
    {
        itemConfig = InteractiveObjectPool.Instance.itemConfigs[ID];
    }
    public void Init()
    {
        _activable = true;
        InteractiveObjectPool.LoadObject(this);
    }
    public virtual void OnTrigger()
    {
        InteractiveObjectPool.ObjectTriggered(this);
        if (count > 0) count--;
    }
    public virtual void EndTrigger(){ }
    public virtual void TriggerEvent(){ }
    public virtual void Activate()
    {
        if (count != 0)
        {
            Sprite sprite = Resources.Load<Sprite>(itemConfig.OutlinedSpritePath) as Sprite;
            spriteRenderer.sprite = sprite;
            Debug.Log(transform.name + " activate" + " change to " + itemConfig.OutlinedSpritePath);
        }
    }
    public virtual void Deactivate()
    {
        Sprite sprite = Resources.Load<Sprite>(itemConfig.UnoutlinedSpritePath) as Sprite;
        spriteRenderer.sprite = sprite;
        Debug.Log(transform.name + " deactivate" + " change to " + itemConfig.UnoutlinedSpritePath);
    }
    private void OnEnable()
    {
        LoadConfig();
        InteractiveObjectPool.LoadObject(this);
    }
    private void OnDisable()
    {
        InteractiveObjectPool.RemoveObject(this);
    }
    private void OnDestroy()
    {
        //InteractiveObjectPool.RemoveObject(this);
    }
}
