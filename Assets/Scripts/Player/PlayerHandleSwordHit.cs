using UnityEngine;

// For easy testing when switching 
// between methods during runtime
public enum DetectMethod
{
    BoxCast,
    OnTriggerEnter
}

public class PlayerHandleSwordHit : MonoBehaviour
{
    public float maxDistance;
    public float moveSpeed;
    public Vector3 drawBoxSize;
    public DetectMethod detectMethod;
    [Space]

    [SerializeField] private Transform raycastTransform;
    [SerializeField] private BoxCollider swordCollider;
    [SerializeField] private Rigidbody swordRigidbody;

    bool hitDetectSide;
    RaycastHit rayHit;
    TagHandle enemyTag;
    Vector3 lastPos;

    void Start()
    {
        enemyTag = TagHandle.GetExistingTag("Enemy");
        detectMethod = DetectMethod.BoxCast;
        lastPos = swordRigidbody.position;
    }

    void FixedUpdate()
    {
        MoveLooping();
        
        if (detectMethod == DetectMethod.BoxCast)
        {
            DetectHitUsingBoxCast();
        }
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

    void OnTriggerEnter(Collider other)
    {
        if (detectMethod == DetectMethod.BoxCast)
        {
            return;
        }

        if (other.CompareTag(enemyTag))
        {
            Debug.Log("Hit : " + other.gameObject.name);
        }
    }

    private void DetectHitUsingBoxCast()
    {
        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data
        hitDetectSide = Physics.BoxCast(swordCollider.bounds.center,
            raycastTransform.localScale * 0.5f,
            raycastTransform.right,
            out rayHit,
            raycastTransform.rotation,
            maxDistance);

        if (hitDetectSide)
        {
            //Output the name of the Collider your Box hit
            Debug.Log("Hit : " + rayHit.collider.name);
        }
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) 
            return;

        //Check if there has been a hit yet
        float castDistanceSide = hitDetectSide ? rayHit.distance : maxDistance;
        
        Gizmos.color = Color.red;
        DrawBoxCast(castDistanceSide, raycastTransform.position, raycastTransform.right);
    }

    private void DrawBoxCast(float drawDistance, Vector3 drawPosition, Vector3 drawDirection)
    {
        //Draw a Ray forward from GameObject toward the hit
        Gizmos.DrawRay(drawPosition, drawDirection * drawDistance);
        //Draw a cube that extends to where the hit exists
        Gizmos.DrawWireCube(drawPosition + drawDirection * drawDistance, drawBoxSize);
    }
}
