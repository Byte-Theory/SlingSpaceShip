using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public bool isMouseLeftTapHold;
    public Vector2 mouseStartPosition;
    public Vector2 mouseEndPosition;
    
    InputAction mousePositionAction;
    InputAction mouseLeftTapHoldAction;

    // Actions
    public static Action UserTapStarted;
    public static Action<Vector2> UserTapped;
    public static Action<Vector2, float> UserTapEnded;
    
    // Ref
    private GameTimerManager gameTimerManager;
    
    private void Start()
    {
        gameTimerManager = GameTimerManager.Instance;
        
        mousePositionAction = InputSystem.actions.FindAction("Mouse Position");
        mouseLeftTapHoldAction = InputSystem.actions.FindAction("Mouse Left Tap");
    }

    private void Update()
    {
        if (mouseLeftTapHoldAction.WasPressedThisFrame())
        {
            isMouseLeftTapHold = true;
            mouseStartPosition = mousePositionAction.ReadValue<Vector2>();
            
            UserTapStarted?.Invoke();
            
            gameTimerManager.SetSlowTimeScale();
        }

        if (mouseLeftTapHoldAction.IsPressed())
        {
            isMouseLeftTapHold = true;
            mouseEndPosition = mousePositionAction.ReadValue<Vector2>();
            
            Vector2 dir = mouseEndPosition - mouseStartPosition;
            dir.Normalize();
            
            UserTapped?.Invoke(dir);
        }
        
        if (mouseLeftTapHoldAction.WasReleasedThisFrame())
        {
            isMouseLeftTapHold = false;
            mouseEndPosition = mousePositionAction.ReadValue<Vector2>();
            
            Vector2 dir = mouseEndPosition - mouseStartPosition;
            float delta = dir.magnitude;
            dir.Normalize();
            
            UserTapEnded?.Invoke(dir, delta);
            
            gameTimerManager.SetNormalTimeScale();
        }
    }
}
