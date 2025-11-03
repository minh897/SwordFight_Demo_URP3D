using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed;
    [SerializeField] private float rotateSpeed;

    private Transform followTarget;
    private Rigidbody _rb;

    void Awake()
    {
        var tmp = FindFirstObjectByType<PlayerInputHandler>();
        followTarget = tmp.gameObject.transform;

        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // following logic
        Vector3 targetPosition = followTarget.position - _rb.position;
        _rb.MovePosition(_rb.position + followSpeed * Time.fixedDeltaTime * targetPosition);

        // rotating logic
        // enemy can only rotate smoothly along the horizontal axis
        // give it a more natural feel than just move to target rotation immediately
        var lookDirection = followTarget.position - _rb.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        float t = Mathf.Clamp01(Time.fixedDeltaTime * rotateSpeed);
        float yRot = Mathf.LerpAngle(transform.localEulerAngles.y, targetRotation.eulerAngles.y, t);
        Quaternion newRotation = Quaternion.Euler(0, yRot, 0);
        _rb.MoveRotation(newRotation);
    }
}
