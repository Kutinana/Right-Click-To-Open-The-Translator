using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Data/PlayerState/FallDown", fileName = "PlayerState_FallDown")]
public class PlayerState_FallDown : PlayerState
{
    [SerializeField] float deceleration = 10f;
    private float LerpParameter;

    protected override void StateInitialization()
    {
        InitialSpeed = playerController.moveSpeed;
        LerpParameter = 0;
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
            stateMachine.SwitchState(typeof(PlayerState_Idle));
        }
        currentSpeed = Mathf.Lerp(InitialSpeed, 0, LerpParameter);
        LerpParameter = LerpParameter <= 1 ? LerpParameter + deceleration * Time.deltaTime : 1;
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        playerController.MoveWithoutPlayerInput(currentSpeed);
        base.PhysicsUpdate();
    }
}