using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class interactiveplayerhint: InteractiveObject
{
    private ItemType itemType;
    public UnityEvent OnT;
    private int animatorHash;
    public string animationName;
    public override void LoadConfig()
    {
        base.LoadConfig();
        this.animatorHash = Animator.StringToHash(animationName);
        if (animator != null) animator.enabled = false;
        
        //animator = GameObject.Find("PlayerHint").GetComponent<Animator>();
        //
        //if (animator != null) animator.enabled = false;
    }

    public override void Activate()
    {
        PlayAnimation();
    }

    public override void Deactivate()
    {
        AnimatorDisabled();
        base.Deactivate();
        //GameObject.Find("PlayerHint").GetComponent<Image>().sprite = null;
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