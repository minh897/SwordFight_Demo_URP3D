using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 5.0f;

    private PlayerInputHandler m_inputHandler;
    private Rigidbody m_rb;
    
    void Start()
    {
        m_inputHandler = GetComponent<PlayerInputHandler>();
        m_rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // the player should move in the direction relative to the main camera
        // for example: the camera is pointing downward from uphigh, the player is 
        // is the camera's center, if the player moves forward their direction should 
        // point toward the upper bound of the camera and if they move bacward the lower bound
        // while still on a horizontal planee disregard their world-space position.

        // read input from PlayerInputHandler
        Vector2 moveInput = m_inputHandler.MoveInput;
        Vector3 moveDir = new(moveInput.x, 0, moveInput.y);

        // move the player rigidbody via Rigidbody.MovePosition()
        Vector3 targetPosition = m_rb.position + moveSpeed * Time.deltaTime * moveDir;
        m_rb.MovePosition(targetPosition);

        // rotate the player facing moving direction smoothly
        // stop the rotation when the direction magnitude get smaller
        if (moveDir.sqrMagnitude <= 0.0001f) return; // this allow more control of the rotation
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        Quaternion newRotation = Quaternion.Lerp(m_rb.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        m_rb.MoveRotation(newRotation);
    }

    void OnDrawGizmos()
    {
        // stop drawing Gizmos when not playing
        if (!Application.isPlaying) return;
        // draw the arrow point to the direction of input not the velocity
        float arrowLength = 1.0F;
        var position = transform.position;

        // read input from PlayerInputHandler
        Vector2 moveInput = m_inputHandler.MoveInput;
        Vector3 moveDir = new(moveInput.x, 0, moveInput.y);
        Quaternion newRot = Quaternion.LookRotation(moveDir);

        // I need to interpolate between current rotation and new rotation
        // to create a smooth rotation, then update the arrow rotation
        // What I need: the current rotation and the new rotation that rotate toward the moving direction

        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, position, newRot, arrowLength, EventType.Repaint);
    }

    // Smoothly rotate the enemy game object to face the given target position
    private void FaceTarget(Vector3 targetPos)
    {
        // Calculate the direction from current position to next target
        Vector3 directionToTarget = targetPos - transform.position;

        // Ignore any difference in vertical position
        directionToTarget.y = 0;

        // Create a new rotation that points the forward vector (y) of the game object up to the calculated direction
        // Since we ignore the vertical position above, the game object can only rotate horizontally
        Quaternion newRot = Quaternion.LookRotation(directionToTarget);

        // Create a smooth rotation from the current rotation to the target rotation at a defined speed
        // Time.deltaTime makes this framerate independent
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, rotateSpeed * Time.deltaTime);
    }
}
