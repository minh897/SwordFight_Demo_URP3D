using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offsetDistance;
    [SerializeField] private float followSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 posWithOffset = offsetDistance + followTarget.position;
        transform.position = Vector3.Lerp(transform.position, posWithOffset, followSpeed * Time.smoothDeltaTime);
    }

    
}
