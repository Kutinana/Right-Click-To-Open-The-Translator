using Puzzle;
using QFramework;
using Translator;
using UI.Narration;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D mrigidbody;
    Animator animator;
    SpriteRenderer spriteRenderer;
    ObjectsDetector objectsDetector;
    GroundDetector groundDetector;

    bool flag = false;
    int par = 0;
    float CurrentMaxSpeed;

    public bool EnableGroundCheck = true;
    public bool touchable => objectsDetector.touchable;
    public float moveSpeed => Mathf.Abs(mrigidbody.velocity.x);
    public void SetCurrentMaxSpeed(float speed) => CurrentMaxSpeed = speed;
    public float GetCurrentMaxSpeed() => CurrentMaxSpeed;
    private void Awake()
    {
        this.mrigidbody = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.objectsDetector = GetComponent<ObjectsDetector>();
        this.animator = GetComponent<Animator>();
        this.groundDetector = GetComponent<GroundDetector>();

        TypeEventSystem.Global.Register<OnPuzzleInitializedEvent>(e =>
        {
            mrigidbody.simulated = false;
            PlayerInput.Instance.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnPuzzleExitEvent>(e =>
        {
            if (CanDeactive)
            {
                mrigidbody.simulated = true;
                PlayerInput.Instance.EnableInputActions();
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e =>
        {
            mrigidbody.simulated = false;
            PlayerInput.Instance.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e =>
        {
            if (CanDeactive)
            {
                mrigidbody.simulated = true;
                PlayerInput.Instance.EnableInputActions();
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnNarrationStartEvent>(e =>
        {
            mrigidbody.simulated = false;
            PlayerInput.Instance.DisableInputActions();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Global.Register<OnNarrationEndEvent>(e =>
        {
            if (CanDeactive)
            {
                mrigidbody.simulated = true;
                PlayerInput.Instance.EnableInputActions();
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
    private void Start()
    {
        PlayerInput.Instance.EnableInputActions();
        PlayerInput.Instance.AddInteractEvent(Interact);
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
        if (PlayerInput.Instance.Move.x != 0) spriteRenderer.flipX = PlayerInput.Instance.Move.x < 0;
        if (PlayerInput.Instance.Move.x > 0) par = 1;
        else if (PlayerInput.Instance.Move.x < 0) par = -1;
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
        && TranslatorSM.StateMachine.CurrentStateId == Translator.States.Off
        && !NarrationManager.IsNarrating;
}