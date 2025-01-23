using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static TouchInput;

[CreateAssetMenu(fileName = "InputReader", menuName = "Swipe/InputReader")]
public class InputReader : ScriptableObject, ITouchMapActions
{
    public event UnityAction ClickStart = delegate { };
    public event UnityAction ClickEnd = delegate { };
    
    private TouchInput playerInput;

    public Vector3 Position => playerInput.TouchMap.TouchedPosition.ReadValue<Vector2>();
    
    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new TouchInput();
            playerInput.TouchMap.SetCallbacks(this);
        }
        
    }
    
    /*private void OnDisable()
    {
        playerInput.Disable();
    }*/
    
    public void EnablePlayerActions()
    {
        playerInput.Enable();
    }
    

    public void OnTouched(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                ClickStart.Invoke();
                break;
            case InputActionPhase.Canceled:
                ClickEnd.Invoke();
                break;
        }
    }

    public void OnTouchedPosition(InputAction.CallbackContext context)
    {
        //noop
    }
}
