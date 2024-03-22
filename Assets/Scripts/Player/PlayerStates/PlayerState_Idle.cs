using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Data/PlayerState/Idle", fileName = "PlayerState_Idle")]
public class PlayerState_Idle : PlayerState
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
        base.Enter();
    }
    public override void LogicUpdate()
    {
        if(playerInput.StartRunning)
        {
            playerInput.isRunning = true;
        }
        if(playerInput.isMoving && !playerInput.isRunning)
        {
            stateMachine.SwitchState(typeof(PlayerState_Walking));
        }
        if(playerInput.isMoving && playerInput.isRunning)
        {
            stateMachine.SwitchState(typeof(PlayerState_Running));
        }
        currentSpeed = Mathf.Lerp(InitialSpeed, 0, LerpParameter);
        LerpParameter = LerpParameter <= 1 ? LerpParameter + deceleration * Time.deltaTime : 1;
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        playerController.Move(currentSpeed);
        base.PhysicsUpdate();
    }
}