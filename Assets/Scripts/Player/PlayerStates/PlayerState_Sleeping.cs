using QFramework;
using UI.Narration;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Data/PlayerState/Sleeping", fileName = "PlayerState_Sleeping")]
public class PlayerState_Sleeping : PlayerState
{
    [SerializeField] protected string animationName2;
    private int animatorHash2;
    protected override void StateInitialization()
    {
        base.StateInitialization();
    }
    public override void Enter()
    {
        animatorHash2 = Animator.StringToHash(animationName2);
        StateInitialization();
        playerInput.DisableInputActions();
        base.Enter();
    }
    public override void Exit()
    {
        playerInput.EnableInputActions();
        base.Exit();
    }
    public override void LogicUpdate()
    {
        if(Input.anyKeyDown)
        {
            TypeEventSystem.Global.Send<OnInitialNarrationStartEvent>();
            stateMachine.SwitchState(typeof(PlayerState_Middle));
        }
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}