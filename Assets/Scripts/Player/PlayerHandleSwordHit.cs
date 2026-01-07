using UnityEngine;

// For easy testing when switching 
// between methods during runtime
public enum DetectMethod
{
    BoxCast,
    OnTriggerEnter,
    OverlapBox,
}

public class PlayerHandleSwordHit : MonoBehaviour
{
    public float maxCastDistance;
    public float moveSpeed;
    public Vector3 drawBoxSize;
    public DetectMethod detectMethod;
    public LayerMask layerMask;
    [Space]

    [SerializeField] private Transform raycastTransform;
    [SerializeField] private BoxCollider swordCollider;
    [SerializeField] private Rigidbody swordRigidbody;

    bool hitDetectSide;
    RaycastHit rayHit;
    Vector3 lastPos;

    void Start()
    {
        lastPos = swordRigidbody.position;
    }

    void FixedUpdate()
    {
        MoveLooping();
        
        if (detectMethod.Equals(DetectMethod.BoxCast))
            DetectHitUsingBoxCast();
        else if (detectMethod.Equals(DetectMethod.OverlapBox))
            DetectHitUsingOverlapBox();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!detectMethod.Equals(DetectMethod.OnTriggerEnter))
            return;

        Debug.Log("OnTriggerEnter Hit : " + other.gameObject.name);
    }

    private void MoveLooping()
    {
        // Move the sword forward using Rigidbody
        Vector3 moveDir = new(0, 0, 1.0f);
        swordRigidbody.MovePosition(swordRigidbody.position + (moveSpeed * Time.fixedDeltaTime * moveDir));

        // Loop back to the started position when exceed certain threshold
        if (swordRigidbody.position.z >= 9.0f)
        {
            swordRigidbody.MovePosition(lastPos);
        }
    }

    private void DetectHitUsingBoxCast()
    {
        // Test to see if there is a hit using a BoxCast
        // Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), 
        // half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        // Also fetch the hit data
        hitDetectSide = Physics.BoxCast(swordCollider.bounds.center,
            raycastTransform.localScale * 0.5f,
            raycastTransform.right,
            out rayHit,
            raycastTransform.rotation,
            maxCastDistance);

        if (hitDetectSide)
        {
            //Output the name of the Collider your Box hit
            Debug.Log("Boxcast Hit : " + rayHit.collider.name);
        }
    }

    private void DetectHitUsingOverlapBox()
    {
        // Use the OverlapBox to detect if there are any other colliders within this box area.
        // Use the GameObject's center, half the size (as a radius), and rotation. 
        // This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(
            raycastTransform.position, 
            raycastTransform.localScale / 2, 
            raycastTransform.localRotation, 
            layerMask);

        int i = 0;
        // Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            // Output all of the collider names
            Debug.Log("OverlapBox Hit : " + hitColliders[i].name + i);
            // Increase the number of Colliders in the array
            i++;
        }
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) 
            return;

        if (detectMethod.Equals(DetectMethod.BoxCast))
        {  
            // Check if there has been a hit yet
            // Then draw a ray according to the hit distance
            float castDistance = hitDetectSide ? rayHit.distance : maxCastDistance;
            DrawBoxCast(castDistance, raycastTransform.position, raycastTransform.right, swordCollider.size);
        }

        if (detectMethod.Equals(DetectMethod.OverlapBox))
        {
            // Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            DrawOverlapBox(raycastTransform.position, swordCollider.size);
        }
    }

    private void DrawBoxCast(float distance, Vector3 position, Vector3 direction, Vector3 scale)
    {
        Gizmos.color = Color.red;
        //Draw a Ray forward from GameObject toward the hit
        Gizmos.DrawRay(position, direction * distance);
        //Draw a cube that extends to where the hit exists
        Gizmos.DrawWireCube(position + direction * distance, scale);
    }

    private void DrawOverlapBox(Vector3 position, Vector3 scale)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(position, scale);
    }
}
