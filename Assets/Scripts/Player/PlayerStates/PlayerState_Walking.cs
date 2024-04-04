
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Data/PlayerState/Walking", fileName = "PlayerState_Walking")]
public class PlayerState_Walking: PlayerState
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float acceleration = 2f;

    private float LerpParameter;

    protected override void StateInitialization()
    {
        InitialSpeed = playerController.moveSpeed;
        playerController.SetCurrentMaxSpeed(walkSpeed);
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
        if (playerInput.StartRunning)
        {
            playerInput.isRunning = true;
        }
        if (playerInput.StartRunning && playerInput.isMoving)
        {
            stateMachine.SwitchState(typeof(PlayerState_Running));
        } 
        if (!playerInput.isMoving)
        {
            stateMachine.SwitchState(typeof(PlayerState_Idle));
        }
        currentSpeed = Mathf.Lerp(InitialSpeed, walkSpeed, LerpParameter);
        LerpParameter = LerpParameter<=1? LerpParameter + acceleration *Time.deltaTime: 1;
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        playerController.Move(currentSpeed);
        base.PhysicsUpdate();
    }
    public float GetMaxSpeed() => walkSpeed;
}