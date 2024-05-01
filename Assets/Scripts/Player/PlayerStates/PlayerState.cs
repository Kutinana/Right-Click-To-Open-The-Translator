using UnityEditorInternal;
using UnityEngine;

public enum StateType
{
    Idle,
    Running,
}

public class PlayerState : ScriptableObject, IState
{
    protected Animator animator;
    protected PlayerStateMachine stateMachine;
    protected PlayerInput playerInput;
    protected PlayerController playerController;

    protected float InitialSpeed;
    protected float currentSpeed;
    protected int animatorHash;
    protected float startTick;
    protected float duration => Time.time - startTick;
    protected bool isAnimationFinished => duration >= animator.GetCurrentAnimatorStateInfo(0).length;
    [SerializeField] protected string animationName;
    [SerializeField, Range(0f, 1f)] float transitionDuration = 0.1f;
    public virtual void Init(PlayerStateMachine stateMachine, Animator animator, PlayerInput playerInput, PlayerController playerController)
    {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.playerInput = playerInput;
        this.playerController = playerController;

        this.animatorHash = Animator.StringToHash(animationName); 
    }
    protected virtual void StateInitialization() { }
    public virtual void Enter()
    {
        animator.CrossFade(animatorHash, transitionDuration);
        startTick = Time.time;
    }

    public virtual void Exit()
    {
        stateMachine.SetLastState(this);
        //Debug.Log("Last state: " + stateMachine.lastState);
    }

    public virtual void LogicUpdate()
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        
    }
}