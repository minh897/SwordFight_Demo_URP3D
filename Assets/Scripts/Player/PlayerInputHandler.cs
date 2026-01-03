using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Vector2 MoveInput { get; private set; }
    public bool AttackInput { get; private set; }

    private PlayerInput playerInput;
    private PlayerCombat _combat;

    private InputAction moveAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        _combat = GetComponent<PlayerCombat>();
    }

    void Start()
    {
        InputSystem.actions.Disable();
        playerInput.currentActionMap?.Enable();
        moveAction = playerInput.currentActionMap.FindAction("Move");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackInput = context.ReadValueAsButton();
    }

    public InputAction GetMoveAction() => moveAction;

    public PlayerCombat GetPlayerCombat() => _combat;
}
