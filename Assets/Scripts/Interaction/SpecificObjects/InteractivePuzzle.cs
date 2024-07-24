using DataSystem;
using Puzzle;
using QFramework;
using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InteractivePuzzle: InteractiveObject
{
    private ItemType itemType;
    public UnityEvent OnT;
    private int animatorHash;
    public string animationName;
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
        itemType = itemConfig.itemType;
        if(animationName != "") this.animatorHash = Animator.StringToHash(animationName);

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
            case ItemType.PUZZLE or ItemType.Hint:
                PuzzleManager.Initialize(itemConfig.target_string);
                break;
            case ItemType.NPC:
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

    public void SwitchSceneWithEvent()
    {
        //SceneControl.SceneControl.SwitchSceneWithEvent(DoorConfig.nextSceneName[this.ID], () => StartCoroutine(SceneInitialization.()));
    }

    public void AnimatorDisabled() => animator.enabled = false;

    public void ActivateHint(String hint)
    {
        Color color = GameObject.Find(hint).GetComponent<SpriteRenderer>().color;
        GameObject.Find(hint).GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
        GameObject.Find(hint).GetComponent<InteractivePuzzle>().SetActivable(true);
    }
    public void turnOnLight(Transform Light)
    {
        parameter = 0f;
        Light2D light2D = Light.GetComponent<Light2D>();
        StartCoroutine(TurnOnLight(light2D));
    }
    private IEnumerator TurnOnLight(Light2D Light)
    {
        Light.intensity = 1;
        yield return new WaitForSeconds(0.3f);
        Light.intensity = 0;
        yield return new WaitForSeconds(0.5f);

        Light.intensity = 1;
        yield return new WaitForSeconds(0.2f);
        Light.intensity = 0;
        yield return new WaitForSeconds(0.2f);
        Light.intensity = 1;
        yield return new WaitForSeconds(0.2f);
        Light.intensity = 0;
        yield return new WaitForSeconds(0.5f);

        while (parameter < 0.99f)
        {
            Light.intensity = Mathf.Lerp(0, 1, parameter);
            parameter += Time.deltaTime * 1.5f;
        }
        Light.intensity = 1f;
    }

    public void OpenDoor(Transform door)
    {
        door.GetComponent<InteractiveDoor>().SetActivable(true);
    }
}