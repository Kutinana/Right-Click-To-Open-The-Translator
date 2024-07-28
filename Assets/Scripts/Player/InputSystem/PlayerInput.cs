using UnityEngine;

public class PlayerInput
{
    private static PlayerInput _instance;
    public static PlayerInput Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerInput();
            }
            return _instance;
        }
        private set => _instance = value;
    }
    PlayerInputActions inputActions = new PlayerInputActions();
    public Vector2 Move => inputActions.GamePlay.Move.ReadValue<Vector2>();
    public bool isMoving => Move.x != 0f;
    public bool StartRunning => inputActions.GamePlay.Running.WasPressedThisFrame();
    public bool RunningStop => inputActions.GamePlay.Running.WasReleasedThisFrame();
    public bool Interact => inputActions.GamePlay.Interact.IsPressed();
    public bool MapControl => inputActions.GamePlay.Map.IsPressed();
    public Vector2 DragMap => inputActions.GamePlay.DragMap.ReadValue<Vector2>();                                             
    public bool isRunning { set; get; }

    private bool mRunning = false;
    private bool mInteract = false;
    private bool mMap = false;

    private void Init()
    {
        Instance.AddRunningEvent();
    }
    public void AddRunningEvent()
    {
        if (!mRunning)
        {
            inputActions.GamePlay.Running.canceled += delegate
            {
                isRunning = false;
            };
            mRunning = true;
        }
    }
    public void AddInteractEvent(System.Action action)
    {
        if (!mInteract)
        {
            inputActions.GamePlay.Interact.performed += delegate
            {
                action.Invoke();
            };
            mInteract = true;
        }
    }
    public void AddMapOpeningEvent(System.Action action)
    {
        if (!mMap)
        {
            inputActions.GamePlay.Map.performed += delegate
            {
                action.Invoke();
            };
            mMap = true;
        }
    }
    public void EnableInputActions()
    {
        inputActions.GamePlay.Enable();
        //Cursor.lockState = CursorLockMode.Locked;
    }
    public void DisableInputActions()
    {
        inputActions.GamePlay.Disable();
    }
}