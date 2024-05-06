using Hint;
using JetBrains.Annotations;
using Puzzle;
using QFramework;
using System.Runtime.Serialization.Configuration;
using Translator;
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
    int enableCount = 0;
    int lastEnableCount = 0;
    string lastLayer = "";

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
            if (lastLayer != "puzzle") enableCount += 1;
            lastLayer = "puzzle";
            //mrigidbody.simulated = false;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnPuzzleExitEvent>(e =>
        {
            enableCount -= 1;
            //mrigidbody.simulated = true;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnHintInitializedEvent>(e =>
        {
            if (lastLayer != "hint") enableCount += 1;
            lastLayer = "hint";
            //mrigidbody.simulated = false;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnHintExitEvent>(e =>
        {
            enableCount -= 1;
            //mrigidbody.simulated = true;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e =>
        {
            if (lastLayer != "trans") enableCount += 1;
            lastLayer = "trans";
            //mrigidbody.simulated = false;
            //playerInput.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e =>
        {
            enableCount -= 1;
            //mrigidbody.simulated = true;
            //playerInput.EnableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    private void Start()
    {
        playerInput.EnableInputActions();
        playerInput.AddInteractEvent(delegate () { Interact(); });
    }
    Vector3 destUp;
    private void Update()
    {
        if (lastEnableCount != enableCount)
        {
            //Debug.Log(enableCount);
            if (enableCount > 0)
            {
                mrigidbody.simulated = false;
                playerInput.DisableInputActions();
            }
            else
            {
                mrigidbody.simulated = true;
                playerInput.EnableInputActions();
            }
        }
        lastEnableCount = enableCount;
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
}