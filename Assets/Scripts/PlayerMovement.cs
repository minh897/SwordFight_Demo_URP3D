using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 5.0f;

    private PlayerInputHandler _inputHandler;
    private Rigidbody _rb;

    private Vector3 _moveDir;
    private Vector3 _currentDir;
    private Vector3 _lastDir;
    
    void Start()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // read input from PlayerInputHandler
        _moveDir = new(_inputHandler.MoveInput.x, 0, _inputHandler.MoveInput.y);

        // move the player to target position
        Vector3 targetPosition = _rb.position + moveSpeed * Time.fixedDeltaTime * _moveDir;
        _rb.MovePosition(targetPosition);

        // record the player last position only there are movements
        // allow rotation to continue even when input is zero 
        _currentDir = _moveDir;
        if (_moveDir.sqrMagnitude > 0.0001f) // this allow more control of the rotation
        {
            _lastDir = _currentDir;
        }
        Quaternion targetRotation = Quaternion.LookRotation(_lastDir);
        // smoothly interpolate player rotation toward moving direction
        Quaternion newRotation = Quaternion.Lerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        _rb.MoveRotation(newRotation);

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
