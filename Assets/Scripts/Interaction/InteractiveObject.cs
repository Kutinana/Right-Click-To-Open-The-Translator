using DataSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;

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
    protected Animator animator;

    protected bool _activable = true;
    public int count = -1;
    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        Init();
        TryGetComponent<Animator>(out animator);
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
    public virtual void Init()
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
            //Debug.Log(transform.name + " activate" + " change to " + itemConfig.OutlinedSpritePath);
        }
    }
    public virtual void Deactivate()
    {
        Sprite sprite;
        if (itemConfig.UnoutlinedSpritePath != "None") sprite = Resources.Load<Sprite>(itemConfig.UnoutlinedSpritePath) as Sprite;
        else sprite = null;
        spriteRenderer.sprite = sprite;
        //Debug.Log(transform.name + " deactivate" + " change to " + itemConfig.UnoutlinedSpritePath);
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

    public void SetActivable(bool activate) => _activable = activate;
}
