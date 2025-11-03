using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offsetDistance;
    [SerializeField] private float followSpeed;

    void FixedUpdate()
    {
        // because the player use rigidbody to move, we  need to put camera follow logic
        // in FixedUpdate so it sync up with the player movement (Unity physics update)
        Vector3 posWithOffset = offsetDistance + followTarget.position;
        transform.position = Vector3.Lerp(transform.position, posWithOffset, followSpeed * Time.fixedDeltaTime);
    }

}
