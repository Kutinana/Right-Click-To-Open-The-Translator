using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class InteractiveObjectPool
{
    private static int MAX_CAPACITY = 64;
    private static List<InteractiveObject>[] ObjectPool = new List<InteractiveObject>[MAX_CAPACITY];
    public static InteractiveObject activeObject = null;

    private static System.Action EventList;
    private static void Init()
    {
        
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
        EventTrigger(delegate () { interactiveObject.TriggerEvent(); });
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
        if (activeObject != null && activeObject != interactiveObject) activeObject.Deactivate();
        if (activeObject != interactiveObject)
        {
            activeObject = interactiveObject;
            if (activeObject != null) activeObject.Activate();
        }
    }
}
public interface Interactive
{
    public int ID { get; }
    public void OnTrigger();
    public void EndTrigger();
    public void Activate();
    public void Deactivate();
}
public class InteractiveObject : MonoBehaviour, Interactive
{
    public int identity_number;
    public int ID 
    {
        get { return identity_number; }
    }
    public void Init()
    {
        InteractiveObjectPool.LoadObject(this);
    }
    public virtual void OnTrigger()
    {
        InteractiveObjectPool.ObjectTriggered(this);
    }
    public virtual void EndTrigger(){ }
    public virtual void TriggerEvent(){ }
    public virtual void Activate()
    {
        Debug.Log(transform.name + " activate");
    }
    public virtual void Deactivate()
    {
        Debug.Log(transform.name + " deactivate");
    }
    private void OnEnable()
    {
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
