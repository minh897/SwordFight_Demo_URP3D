using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyHitDetection))]
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followSpeed;
    [SerializeField] private float rotateSpeed;

    private Rigidbody rb;
    private EnemyHitDetection hitDetection;
    private Vector3 lookDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hitDetection = GetComponent<EnemyHitDetection>();

        if (!followTarget)
            followTarget = FindFirstObjectByType<PlayerInputHandler>().transform;
    }

    void FixedUpdate()
    {
        if (followTarget.Equals(null)) return;

        // if enemy is stunned then stop all movement
        if (hitDetection.GetHit) return;

        // continously updating direction
        lookDirection = followTarget.position - transform.position;
        rb.MovePosition(rb.position + followSpeed * Time.fixedDeltaTime * lookDirection);

        // rotate the transform toward the target's direction
        // restrict the direction to horizontal plane only before creating a new rotation
        // check for magnitude to avoid error if the direction Vector3 is zero
        Vector3 flatDirection = lookDirection;
        flatDirection.y = 0f;
        if (flatDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion newRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = newRotation;
        }
    }

    public Vector3 GetMovingDirection() => lookDirection;
}
