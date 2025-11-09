using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 5.0f;

    private PlayerInputHandler inputHandler;
    private Rigidbody rb;

    private Vector3 moveDir;
    private Vector3 currentDir;
    private Vector3 lastDir;
    
    void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody>();

        // if (inputHandler.MoveInput.Equals(Vector2.zero)) Debug.Log("input is zero");
    }

    void FixedUpdate()
    {
        // stop receiving input when attack is performed
        if (inputHandler.GetPlayerCombat().IsAttacking()) return;

        // read input from PlayerInputHandler
        moveDir = new(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);

        // move the player to target position
        Vector3 targetPosition = rb.position + moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.MovePosition(targetPosition);

        // record the player last position only there are movements
        // allow rotation to continue even when input is zero 
        currentDir = moveDir;
        if (moveDir.sqrMagnitude > 0.0001f) // this allow more control of the rotation
        {
            lastDir = currentDir;
        }
        Quaternion targetRotation = Quaternion.LookRotation(lastDir);
        // smoothly interpolate player rotation toward moving direction
        Quaternion newRotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        rb.MoveRotation(newRotation);
    }

    void OnDrawGizmos()
    {
        // stop drawing Gizmos when not playing
        if (!Application.isPlaying) return;

        // draw the arrow point to the direction of input not the velocity
        float arrowLength = 1.0F;
        transform.GetPositionAndRotation(out var position, out var rotation);

        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, position, rotation, arrowLength, EventType.Repaint);
    }
}
