
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Data/PlayerState/Running", fileName = "PlayerState_Running")]
public class PlayerState_Running: PlayerState
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float acceleration = 2f;

    private float LerpParameter;

    protected override void StateInitialization()
    {
        InitialSpeed = playerController.moveSpeed;
        playerController.SetCurrentMaxSpeed(runSpeed);
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
        if (playerInput.RunningStop && playerInput.isMoving)
        {
            stateMachine.SwitchState(typeof(PlayerState_Walking));
        } 
        if (!playerInput.isMoving)
        {
            stateMachine.SwitchState(typeof(PlayerState_Idle));
        }
        currentSpeed = Mathf.Lerp(InitialSpeed, runSpeed, LerpParameter);
        LerpParameter = LerpParameter<=1? LerpParameter + acceleration *Time.deltaTime: 1;
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        playerController.Move(currentSpeed);
        base.PhysicsUpdate();
    }
    public float GetMaxSpeed() => runSpeed;
}