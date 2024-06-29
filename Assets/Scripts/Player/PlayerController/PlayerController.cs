using Hint;
using JetBrains.Annotations;
using Puzzle;
using QFramework;
using System.Runtime.Serialization.Configuration;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody2D mrigidbody;
    Animator animator;
    SpriteRenderer spriteRenderer;
    ObjectsDetector objectsDetector;
    GroundDetector groundDetector;

    bool flag = false;
    int par = 0;
    float CurrentMaxSpeed;
    bool enableCount = false;

    public bool EnableGroundCheck = true;
    public bool touchable => objectsDetector.touchable;
    public float moveSpeed => Mathf.Abs(mrigidbody.velocity.x);
    public void SetCurrentMaxSpeed(float speed) => CurrentMaxSpeed = speed;
    public float GetCurrentMaxSpeed() => CurrentMaxSpeed;
    private void Awake()
    {
        this.playerInput = GetComponent<PlayerInput>();
        this.mrigidbody = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.objectsDetector = GetComponent<ObjectsDetector>();
        this.animator = GetComponent<Animator>();
        this.groundDetector = GetComponent<GroundDetector>();

        TypeEventSystem.Global.Register<OnPuzzleInitializedEvent>(e =>
        {
            enableCount = true;
            mrigidbody.simulated = false;
            playerInput.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnPuzzleExitEvent>(e =>
        {
            if (enableCount && CanDeactive)
            {
                mrigidbody.simulated = true;
                playerInput.EnableInputActions();
                enableCount = false;
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnHintInitializedEvent>(e =>
        {
            enableCount = true;
            mrigidbody.simulated = false;
            playerInput.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnHintExitEvent>(e =>
        {
            if (enableCount && CanDeactive)
            {
                mrigidbody.simulated = true;
                playerInput.EnableInputActions();
                enableCount = false;
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e =>
        {
            enableCount = true;
            mrigidbody.simulated = false;
            playerInput.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e =>
        {
            if (enableCount && CanDeactive)
            {
                mrigidbody.simulated = true;
                playerInput.EnableInputActions();
                enableCount = false;
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnNarrationStartEvent>(e =>
        {
            enableCount = true;
            mrigidbody.simulated = false;
            playerInput.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnNarrationEndEvent>(e =>
        {
            if (enableCount && CanDeactive)
            {
                mrigidbody.simulated = true;
                playerInput.EnableInputActions();
                enableCount = false;
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    private void Start()
    {
        playerInput.EnableInputActions();
        playerInput.AddInteractEvent(delegate () { Interact(); });
        playerInput.AddMapOpeningEvent(delegate () { MapControl(); });
    }

    private void Update()
    {
        ActivateObject();
        while (flag && !groundDetector.isGrounded && EnableGroundCheck)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.01f);
        }
        flag = groundDetector.isGrounded;
    }
    private void ActivateObject()
    {
        InteractiveObject interactiveObject;
        if (touchable)
        {
            interactiveObject = objectsDetector.DetectClosestObject();
            if (interactiveObject == null)
            {
                InteractiveObjectPool.SetActiveObject(null);
                // Debug.Log("Could not find corresponding InteractiveObject");
                return;
            }
            InteractiveObjectPool.SetActiveObject(interactiveObject);
        }
        else
        {
            InteractiveObjectPool.SetActiveObject(null);
        }
    }
    private void Interact()
    {
        if (InteractiveObjectPool.Instance.activeObject == null) return;
        InteractiveObjectPool.Instance.activeObject.OnTrigger();
    }

    private void MapControl()
    {
        if (MapController.Instance.currentMapState == MapController.MapState.Closing) MapController.OpenMap();
        else MapController.CloseMap();
    }
    public void Move(float speed)
    {
        if (playerInput.Move.x != 0) spriteRenderer.flipX = playerInput.Move.x < 0;
        if (playerInput.Move.x > 0) par = 1;
        else if (playerInput.Move.x < 0) par = -1;
        SetVelocityX(speed * par);
    }
    public void MoveWithoutPlayerInput(float speed)
    {
        par = spriteRenderer.flipX ? -1 : 1;
        SetVelocityX(speed * par);
    }
    public void SetVelocity(Vector3 velocity)
    {
        mrigidbody.velocity = velocity;
    }
    public void SetVelocityX(float VelocityX)
    {
        mrigidbody.velocity = new Vector3(VelocityX, mrigidbody.velocity.y);
    }
    public void SetVelocityY(float VelocityY)
    {
        mrigidbody.velocity = new Vector3(mrigidbody.velocity.x, VelocityY);
    }
    public void SetGravity(float value)
    {
        mrigidbody.gravityScale = value;
    } 
    public void PlayFootStep(){
        AudioMng.Instance.PlayFootsteps();
    }
    public void PlaySpinFall1(){
        AudioKit.PlaySound("Spinfall1");
    }
    public void PlaySpinFall2(){
        AudioKit.PlaySound("Spinfall2");
    }

    private bool CanDeactive => PuzzleManager.StateMachine.CurrentStateId == PuzzleManager.States.None
        && HintManager.StateMachine.CurrentStateId == HintManager.States.None
        && TranslatorSM.StateMachine.CurrentStateId == Translator.States.Off
        && !NarrationManager.IsNarrating;
}