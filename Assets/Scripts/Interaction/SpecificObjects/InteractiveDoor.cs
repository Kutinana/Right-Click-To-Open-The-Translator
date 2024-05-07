
using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveDoor: InteractiveObject
{
    public UnityEvent OnT;
    bool startFall = false;
    private void Update()
    {
        if (startFall){
            AudioKit.PlaySound("Fall-Zero",volumeScale:0.8f);
            StartCoroutine(playerFall());}
    }
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
    public void PlayerFall()
    {
        startFall = true;
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

        if (DoorConfig.nextSceneName[this.ID] is not "StartScene")
        {
            GameProgressData.SaveLastScene(DoorConfig.nextSceneName[this.ID]);
        }
    }
    IEnumerator playerFall()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().EnableGroundCheck = false;
        player.GetComponent<Rigidbody2D>().simulated = false;
        player.GetComponent<PlayerInput>().DisableInputActions();

        yield return new WaitForSeconds(0.3f);

        Color color = GameObject.Find("storage_9.2-warpgate").GetComponent<SpriteRenderer>().color;
        GameObject.Find("storage_9.2-warpgate").GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/player_falling") as Sprite;

        yield return new WaitForSeconds(0.7f);
        
        float startTime = Time.time;
        Vector3 position = player.transform.position;
        while (Time.time - startTime < 0.8f)
        {
            player.transform.position = Vector3.Lerp(position, new Vector3(position.x, position.y - 2.0f, position.z), Time.deltaTime * 10);

            yield return null;
        }
    }
    public void SwitchSound ()
    {
        AudioKit.PlaySound("Switch");
    }
}