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

    void Update()
    {
        // stop receiving input when attack is performed
        if (inputHandler.GetPlayerCombat().IsAttacking()) return;

        // read input from PlayerInputHandler
        moveDir = new(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
    }

    void FixedUpdate()
    {
        // move the player to target position
        Vector3 targetPosition = rb.position + moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.MovePosition(targetPosition);

        // record the player last position only there are movements
        // allow rotation to continue even when input is zero 
        currentDir = moveDir;
        // this allow more control of the rotation
        if (moveDir.sqrMagnitude > 0.0001f) lastDir = currentDir; 
        // stop rotation if no movement input have been made, and last direction is unknown
        if (moveDir.Equals(Vector3.zero) && lastDir.Equals(Vector3.zero)) return;
        Quaternion targetRotation = Quaternion.LookRotation(lastDir);
        // smoothly interpolate player rotation toward moving direction
        Quaternion newRotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        rb.MoveRotation(newRotation);
    }

    void OnDrawGizmos()
    {
        // stop drawing Gizmos outside of in Play Mode
        if (!Application.isPlaying) return;

        // draw the arrow point to the direction of input not the velocity
        float arrowLength = 1.0F;
        transform.GetPositionAndRotation(out var position, out var rotation);

        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, position, rotation, arrowLength, EventType.Repaint);
    }
}
