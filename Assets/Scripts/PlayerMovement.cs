using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 5.0f;

    private PlayerInputHandler m_inputHandler;
    private Rigidbody m_rb;

    private Vector3 moveDir;
    private Vector3 currentPos;
    private Vector3 lastPos;
    
    void Start()
    {
        m_inputHandler = GetComponent<PlayerInputHandler>();
        m_rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // read input from PlayerInputHandler
        moveDir = new(m_inputHandler.MoveInput.x, 0, m_inputHandler.MoveInput.y);
        
        // rotate the player facing moving direction smoothly
        // stop the rotation when the direction magnitude get smaller
        currentPos = moveDir;
        if (moveDir.sqrMagnitude > 0.0001f) // this allow more control of the rotation
        {
            lastPos = currentPos;
        } 
        Quaternion targetRotation = Quaternion.LookRotation(lastPos);
        Quaternion newRotation = Quaternion.Lerp(m_rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        m_rb.MoveRotation(newRotation);

        // move the player rigidbody via Rigidbody.MovePosition()
        Vector3 targetPosition = m_rb.position + moveSpeed * Time.fixedDeltaTime * moveDir;
        m_rb.MovePosition(targetPosition);
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
