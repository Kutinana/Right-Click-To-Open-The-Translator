using Hint;
using Puzzle;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class interactiveEhint: InteractiveObject
{
    private ItemType itemType;
    public UnityEvent OnT;
    private int animatorHash;
    public string animationName;
    public override void LoadConfig()
    {
        base.LoadConfig();
        this.animatorHash = Animator.StringToHash(animationName);
        itemType = itemConfig.itemType;
        animator = GetComponentInChildren<Animator>();

        if (animator != null) animator.enabled = false;
        
        //animator = GameObject.Find("PlayerHint").GetComponent<Animator>();
        //
        //if (animator != null) animator.enabled = false;
    }

    public override void Activate()
    {
        base.Activate();
        PlayAnimation();
    }

    public override void Deactivate()
    {
        AnimatorDisabled();
        animator.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        base.Deactivate();
        //GameObject.Find("PlayerHint").GetComponent<Image>().sprite = null;
    }
    public override void TriggerEvent()
    {
        switch (itemType)
        {
            case ItemType.DOOR:
                SwitchScene();
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
    public void ChangeObject(int id)
    {
        Deactivate();
        _activable = false;
        InteractiveObjectPool.RemoveObject(this);
        identity_number = id;
        Init();
        LoadConfig();
    }
    public void PlayAnimation()
    {
        animator.enabled = true;
        animator?.CrossFade(animatorHash, 0.1f);
    }

    public void PlaySound(string name)
    {
        AudioKit.PlaySound(name);
    }

    public void Refresh()
    {
        InteractiveObjectPool.RefreshActiveObject(this);
    }
    public void RefreshAfterSeconds(float t)
    {
        Invoke(nameof(Refresh), t);
    }
    public void SwitchScene()
    {
        SceneControl.SceneControl.SwitchSceneWithoutConfirm(DoorConfig.nextSceneName[this.ID]);
    }

    public void AnimatorDisabled() => animator.enabled = false;
}