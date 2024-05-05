using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Data/PlayerState/FallDown", fileName = "PlayerState_FallDown")]
public class PlayerState_FallDown : PlayerState
{

    private float LerpParameter;

    protected override void StateInitialization()
    {
        base.StateInitialization();
    }
    public override void Enter()
    {
        StateInitialization();
        base.Enter();
    }
    public override void LogicUpdate()
    {
        if(isAnimationFinished)
        {
            stateMachine.SwitchState(typeof(PlayerState_Idle));
        }
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}