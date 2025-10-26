using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Vector2 moveInput;

    private PlayerInput playerInput;
    private PlayerMovement m_Movement;
    private PlayerCombat m_Combat;

    private PlayerInputActions m_Actions;
    private PlayerInputActions.PlayerActions m_Player;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        m_Movement = GetComponent<PlayerMovement>();
        m_Combat = GetComponent<PlayerCombat>();
    }

    void Start()
    {
        InputSystem.actions.Disable();
        playerInput.currentActionMap?.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // specify the context of the input
        if (context.performed)
        {
            Debug.Log("OnMove() was called");
            moveInput = context.ReadValue<Vector2>();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("OnLook() was called");
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnAttack() was called");
        }
    }

}
