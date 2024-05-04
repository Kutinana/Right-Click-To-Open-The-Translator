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
    }
    private void Start()
    {
        playerInput.EnableInputActions();
        playerInput.AddInteractEvent(delegate () { Interact(); });
    }
    Vector3 destUp;
    private void Update()
    {
        ActivateObject();
        //while (flag && !groundDetector.isGrounded && EnableGroundCheck)
        //{
        //    transform.position = new Vector2(transform.position.x, transform.position.y - 0.01f);
        //}
        //flag = groundDetector.isGrounded;
    }
    private void ActivateObject()
    {
        InteractiveObject interactiveObject;
        if (touchable)
        {
            interactiveObject = objectsDetector.DetectClosestObject();
            if (interactiveObject == null)
            {
                Debug.Log("Could not find corresponding InteractiveObject");
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