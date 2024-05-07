using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Data/PlayerState/Middle", fileName = "PlayerState_Middle")]
public class PlayerState_Middle : PlayerState
{

    protected override void StateInitialization()
    {
        base.StateInitialization();
    }
    public override void Enter()
    {
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
        if(isAnimationFinished)
        {
            stateMachine.SwitchState(typeof(PlayerState_Wakeup));
        }
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}