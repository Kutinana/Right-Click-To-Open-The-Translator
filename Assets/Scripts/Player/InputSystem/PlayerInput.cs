using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput: MonoBehaviour
{
    PlayerInputActions inputActions;
    public Vector2 Move => inputActions.GamePlay.Move.ReadValue<Vector2>();
    public bool isMoving => Move.x != 0f;
    public bool StartRunning => inputActions.GamePlay.Running.WasPressedThisFrame();
    public bool RunningStop => inputActions.GamePlay.Running.WasReleasedThisFrame();
    public bool isRunning { set; get; }
    private void Awake()
    {
        this.inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        inputActions.GamePlay.Running.canceled += delegate
        {
            isRunning = false;
        };
    }
    public void EnableInputActions()
    {
        inputActions.GamePlay.Enable();
        //Cursor.lockState = CursorLockMode.Locked;
    }
}