
using System.Collections;
using System.Collections.Generic;
using DataSystem;
using Localization;
using QFramework;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveDoor: InteractiveObject
{
    public UnityEvent OnT;
    public bool setActivable = true;

    float parameter = 0f;

    public override void Init()
    {
        base.Init();
        SetActivable(setActivable);
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
        LightController.LightController.Instance.UpdateLights();
    }
    public void LeaveRoom(GameObject room)
    {
        GameObject.Find("TempPlayer").GetComponent<PlayerController>().EnableGroundCheck = false;
        room.SetActive(false);
        LightController.LightController.Instance.UpdateLights();
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
    Vector3 position;
    GameObject player;
    public void PlayerFall()
    {
        parameter = 0f;
        player = GameObject.FindGameObjectWithTag("Player");
        position = player.transform.position;
        AudioKit.PlaySound("Fall-Zero", volumeScale: 0.5f);
        StartCoroutine(playerFall());
    }
    public void PlayAnimation(string animationName)
    {
        int animatorHash = Animator.StringToHash(animationName);
        animator.enabled = true;
        animator?.CrossFade(animatorHash, 0.1f);
    }
    public void AnimatorDisabled() => animator.enabled = false;
    public void SwitchScene()
    {
        SceneControl.SceneControl.SwitchSceneWithoutConfirm(DoorConfig.nextSceneName[this.ID]);

        if (DoorConfig.nextSceneName[this.ID] is not "StartScene")
        {
            GameProgressData.SaveLastScene(DoorConfig.nextSceneName[this.ID]);
        }
    }
    private IEnumerator playerFall()
    {
        
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
        while (Time.time - startTime < 0.8f)
        {
            player.transform.position = Vector3.Lerp(position, new Vector3(position.x, position.y - 6.0f, position.z), parameter);
            parameter += Time.deltaTime * 10f;

            yield return null;
        }
    }

    public void SwithColor()
    {
        if (GameProgressData.GetInventory().TryGetValue("greenLiquid", out var value) && value >= 1)
        {
            GetObtainableObject("yellowLiquid");
        }
    }

    public void SwitchItem()
    {
        if (GameProgressData.GetInventory().TryGetValue("yellowLiquid", out var value) && value >= 1)
        {
            GetObtainableObject("dirt2");
        }
    }

    public void GetObtainableObject(string name)
    {
        GameProgressData.IncreaseInventory(name);
    }

    public void SwitchSound ()
    {
        AudioKit.PlaySound("Switch");
    }
    public void QuitGame()
    {
        QuitPanelController.StartQuitting();
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
    public void Refresh()
    {
        InteractiveObjectPool.RefreshActiveObject(this);
    }
}