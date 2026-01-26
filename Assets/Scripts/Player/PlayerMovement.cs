using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 5.0f;

    private Rigidbody rb;
    private PlayerInputHandler inputHandler;

    private Vector3 moveDir;
    private Vector3 currentDir;
    private Vector3 lastDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void FixedUpdate()
    {
        // read input from PlayerInputHandler
        moveDir = new(inputHandler.InputMove.x, 0f, inputHandler.InputMove.y);

        // stop movement when attack is performed
        if (inputHandler.GetPlayerCombat().IsAttacking) return;

        // move the transform to target position
        Vector3 targetPosition = rb.position + moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.MovePosition(targetPosition);

        // rotate the transform toward input direction
        // record last position when there are movements
        // allow rotation to continue even when input is zero 
        currentDir = moveDir;
        // this allow more control of the rotation
        if (moveDir.sqrMagnitude > 0.0001f) lastDir = currentDir; 
        // stop rotation if no inputs have been made, and the last direction is unknown
        if (moveDir.Equals(Vector3.zero) && lastDir.Equals(Vector3.zero)) return;
        Quaternion targetRotation = Quaternion.LookRotation(lastDir);
        Quaternion newRotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        rb.MoveRotation(newRotation);
    }
}
