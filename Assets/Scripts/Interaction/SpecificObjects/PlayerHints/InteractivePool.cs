using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class interactivePool: InteractiveObject
{
    private ItemType itemType;
    public UnityEvent OnT;
    private int animatorHash;
    public string animationName;
    float startTime;
    bool startfalldown = false;
    public override void LoadConfig()
    {
        base.LoadConfig();
        this.animatorHash = Animator.StringToHash(animationName);
        
        animator = GameObject.Find("TempPlayer").GetComponent<Animator>();
        //
        //if (animator != null) animator.enabled = false;
    }

    public override void Activate()
    {
        if (count > 0)
        {
            PlayAnimation();
            count--;
        }
        //base.Activate();
    }

    public override void Deactivate()
    {
        //AnimatorDisabled();
        //base.Deactivate();
    }

    private void PlayAnimation()
    {
        GameObject.Find("TempPlayer").GetComponent<PlayerStateMachine>().SwitchState(typeof(PlayerState_FallDown));
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

    public void AnimatorDisabled() => animator.enabled = false;
}