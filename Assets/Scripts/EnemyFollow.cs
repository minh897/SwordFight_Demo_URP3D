using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyHitDetection))]
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed;
    [SerializeField] private float rotateSpeed;

    private Transform followTarget;
    private Rigidbody _rb;
    private EnemyHitDetection hitDetection;
    private Vector3 direction;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        hitDetection = GetComponent<EnemyHitDetection>();
        var tmp = FindFirstObjectByType<PlayerInputHandler>();
        followTarget = tmp.gameObject.transform;
    }

    void Update()
    {
        // continously updating direction
        direction = followTarget.position - _rb.position;
    }

    void FixedUpdate()
    {
        // if enemy is stunned then stop all movement
        if (hitDetection.IsStunned()) return;

        // enemy can only rotate smoothly along the horizontal axis
        // give it a more natural feel than just move to target rotation immediately
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float t = Mathf.Clamp01(Time.fixedDeltaTime * rotateSpeed);
        float yRot = Mathf.LerpAngle(transform.localEulerAngles.y, targetRotation.eulerAngles.y, t);
        Quaternion newRotation = Quaternion.Euler(0, yRot, 0);

        _rb.MovePosition(_rb.position + followSpeed * Time.fixedDeltaTime * direction);
        _rb.MoveRotation(newRotation);
    }

    public Vector3 GetMovingDirection() => direction;
}
