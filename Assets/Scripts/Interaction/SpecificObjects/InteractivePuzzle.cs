using Hint;
using Puzzle;
using QFramework;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class InteractivePuzzle: InteractiveObject
{
    private ItemType itemType;
    public UnityEvent OnT;
    private int animatorHash;
    public string animationName;

    public override void LoadConfig()
    {
        base.LoadConfig();
        itemType = itemConfig.itemType;
        this.animatorHash = Animator.StringToHash(animationName);
        animator.enabled = false;
    }
    public override void TriggerEvent()
    {
        switch (itemType)
        {
            case ItemType.DOOR:
                break;
            case ItemType.PUZZLE:
                PuzzleManager.Initialize(itemConfig.target_string);
                break;
            case ItemType.NPC:
                break;
            case ItemType.Hint:
                HintManager.Initialize(itemConfig.target_string);
                break;
            default:
                break;
        }
        //GameObject.Find("TempPlayer").GetComponent<PlayerInput>().DisableInputActions();
        //GameObject.Find("TempPlayer").GetComponent<Rigidbody2D>().simulated = false;
        base.TriggerEvent();
    }
    public override void EndTrigger()
    {
        GameObject.Find("TempPlayer").GetComponent<Rigidbody2D>().simulated = true;
        base.EndTrigger();
    }
    public void ChangeObject(int id)
    {
        Deactivate();
        _activable = false;
        InteractiveObjectPool.RemoveObject(this);
        identity_number = id;
        Init();
        LoadConfig();
        animator.enabled = true;
        animator?.CrossFade(animatorHash, 0.1f);
    }

    public void PlaySound(string name)
    {
        AudioKit.PlaySound(name);
    }
    
    public void AnimatorDisabled() => animator.enabled = false;
}