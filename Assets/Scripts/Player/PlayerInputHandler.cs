using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Vector2 InputMove { get; private set; }
    public bool InputAttack { get; private set; }

    private PlayerInput playerInput;
    private PlayerCombat playerCombat;

    private InputAction actionMovement;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    void Start()
    {
        InputSystem.actions.Disable();
        playerInput.currentActionMap?.Enable();
        actionMovement = playerInput.currentActionMap.FindAction("Move");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        InputMove = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        InputAttack = context.ReadValueAsButton();
    }

    public InputAction GetMoveAction() => actionMovement;

    public PlayerCombat GetPlayerCombat() => playerCombat;
}
