using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, PlayerInputActions.IPlayerActions
{
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
        var value = context.ReadValue<Vector2>();
        Debug.Log("Movement value: " + value);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("OnLook() was called");
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("OnAttack() was called");
    }

}
