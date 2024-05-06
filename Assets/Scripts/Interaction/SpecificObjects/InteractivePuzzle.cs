using DataSystem;
using Hint;
using Puzzle;
using QFramework;
using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class InteractivePuzzle: InteractiveObject
{
    private ItemType itemType;
    public UnityEvent OnT;
    private int animatorHash;
    public string animationName;
    public bool setActivable = true;

    public override void LoadConfig()
    {
        base.LoadConfig();
        itemType = itemConfig.itemType;
        this.animatorHash = Animator.StringToHash(animationName);
        SetActivable(setActivable);

        if (animator != null) animator.enabled = false;
    }
    private void Start()
    {
        if (GameProgressData.GetPuzzleProgress(itemConfig.target_string) == PuzzleProgress.Solved)
        {
            OnT.Invoke();
        }
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

    public void ActivateHint(String hint)
    {
        Color color = GameObject.Find(hint).GetComponent<SpriteRenderer>().color;
        GameObject.Find(hint).GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
        GameObject.Find(hint).GetComponent<InteractivePuzzle>().SetActivable(true);
    }
}