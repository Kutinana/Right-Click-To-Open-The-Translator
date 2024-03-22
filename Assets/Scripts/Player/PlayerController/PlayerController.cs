using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody2D mrigidbody;
    SpriteRenderer spriteRenderer;
    int par = 0;

    public float moveSpeed => Mathf.Abs(mrigidbody.velocity.x);
    private void Awake()
    {
        this.playerInput = GetComponent<PlayerInput>();
        this.mrigidbody = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        playerInput.EnableInputActions();
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
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