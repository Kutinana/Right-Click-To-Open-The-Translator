using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody2D mrigidbody;
    SpriteRenderer spriteRenderer;
    ObjectsDetector objectsDetector;
    int par = 0;

    public bool touchable => objectsDetector.touchable;
    public float moveSpeed => Mathf.Abs(mrigidbody.velocity.x);
    private void Awake()
    {
        this.playerInput = GetComponent<PlayerInput>();
        this.mrigidbody = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.objectsDetector = GetComponent<ObjectsDetector>();
    }
    private void Start()
    {
        playerInput.EnableInputActions();
        playerInput.AddInteractEvent(delegate () { Interact(); });
    }
    private void Update()
    {
        ActivateObject();
    }
    private void FixedUpdate()
    {
        
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
        if (InteractiveObjectPool.activeObject == null) return;
        InteractiveObjectPool.activeObject.OnTrigger();
    }
    public void Move(float speed)
    {
        if (speed != 0) spriteRenderer.flipX = playerInput.Move.x < 0;
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
    public void SetGravity(float value)
    {
        mrigidbody.gravityScale = value;
    } 
}