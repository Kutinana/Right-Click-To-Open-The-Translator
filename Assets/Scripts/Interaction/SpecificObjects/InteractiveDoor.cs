
using UnityEngine;
using UnityEngine.Events;

public class InteractiveDoor: InteractiveObject
{
    public UnityEvent OnT;
    public override void LoadConfig()
    {
        base.LoadConfig();
        if (animator != null) animator.enabled = false;

        //animator = GameObject.Find("PlayerHint").GetComponent<Animator>();
        //
        //if (animator != null) animator.enabled = false;
    }
    public override void TriggerEvent()
    {
        OnT.Invoke();
        base.TriggerEvent();
    }
    public void SwitchRoom(GameObject room)
    {
        room.SetActive(true);
    }
    public void LeaveRoom(GameObject room)
    {
        GameObject.Find("TempPlayer").GetComponent<PlayerController>().EnableGroundCheck = false;
        room.SetActive(false);
    }
    public void SetCameraMinX(int range)
    {
        CameraFollowController camera = GameObject.Find("Main Camera").GetComponent<CameraFollowController>();
        camera.minX = range;
    }
    public void SetCameraMaxX(int range)
    {
        CameraFollowController camera = GameObject.Find("Main Camera").GetComponent<CameraFollowController>();
        camera.maxX = range;
    }
    public void SetAnimator(GameObject gameObject)
    {
        animator = gameObject.GetComponent<Animator>();
    }
    public void WaitForSecondToSwitch(float t)
    {
        Invoke(nameof(SwitchScene), t);
    }
    public void WaitForSecondToDisable(float t)
    {
        Invoke(nameof(AnimatorDisabled), t);
    }
    public void PlayAnimation(string animationName)
    {
        int animatorHash = Animator.StringToHash(animationName);
        animator.enabled = true;
        animator?.CrossFade(animatorHash, 0.1f);
    }
    public void AnimatorDisabled() => animator.enabled = false;
    private void DoNothing() { }
    public void SwitchScene()
    {
        SceneControl.SceneControl.SwitchSceneWithoutConfirm(DoorConfig.nextSceneName[this.ID]);
    }
}