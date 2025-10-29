using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Vector2 MoveInput { get; private set; }
    public bool AttackInput { get; private set; }

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
        MoveInput = context.ReadValue<Vector2>();
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackInput = context.ReadValueAsButton();
    }

}
