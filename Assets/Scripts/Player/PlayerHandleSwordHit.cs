using System.Collections.Generic;
using UnityEngine;

// For easy testing when switching 
// between methods during runtime
public enum DetectMethod
{
    BoxCastAll,
    OnTriggerEnter,
    OverlapBox,
}

public class PlayerHandleSwordHit : MonoBehaviour
{
    public float maxCastDistance;
    public float moveSpeed;
    public int damage;
    public Vector3 drawBoxSize;
    public DetectMethod detectMethod;
    public LayerMask layerMask;
    [Space]

    [SerializeField] private Transform raycastTransform;
    [SerializeField] private BoxCollider swordCollider;
    [SerializeField] private Rigidbody swordRigidbody;

    bool hitDetect;
    float lastZPos;
    RaycastHit rayHit;
    
    // Prevent multiple hits against the same target
    private HashSet<EnemyHitDetection> hitThisSwing;

    void Start()
    {
        lastZPos = swordRigidbody.position.z;
        hitThisSwing = new();
    }

    void OnDisable()
    {
        hitThisSwing.Clear();
        enabled = true;
    }

    void FixedUpdate()
    {
        MoveLooping();

        if (detectMethod.Equals(DetectMethod.BoxCastAll))
            DetectHitUsingBoxCastAll();
        else if (detectMethod.Equals(DetectMethod.OverlapBox))
            DetectHitUsingOverlapBox();
    }

#region Hit Detection Methods
    void OnTriggerEnter(Collider other)
    {
        if (!detectMethod.Equals(DetectMethod.OnTriggerEnter))
            return;

        // Debug.Log("OnTriggerEnter Hit : " + other.gameObject.name);
        if (other.TryGetComponent<EnemyHitDetection>(out var target))
        {
            DeliverHitToTarget(target, other);
        }
    }

    private void DetectHitUsingBoxCastAll()
    {
        // Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), 
        // half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        // Also fetch the hit data
        RaycastHit[] hitRays = Physics.BoxCastAll(
            swordCollider.bounds.center,
            raycastTransform.localScale * 0.5f,
            raycastTransform.right,
            raycastTransform.rotation,
            maxCastDistance, 
            layerMask);

        hitDetect = hitRays.Length > 0;

        int i = 0;
        // Check when there is a new collider coming into contact with the box
        while (i < hitRays.Length)
        {
            // Debug.Log("BoxCastAll Hit : " + hitRays[i].collider.name);
            if (hitRays[i].collider.TryGetComponent<EnemyHitDetection>(out var target))
            {
                DeliverHitToTarget(target, hitRays[i].collider);
            }
            i++;
        }
    }

    private void DetectHitUsingOverlapBox()
    {
        // Use the OverlapBox to detect if there are any other colliders within this box area.
        // Use the GameObject's center, half the size (as a radius), and rotation. 
        // This creates an invisible box around your GameObject.
        Collider[] overlapColliders = Physics.OverlapBox(
            raycastTransform.position, 
            raycastTransform.localScale / 2, 
            raycastTransform.localRotation, 
            layerMask);

        int i = 0;
        // Check when there is a new collider coming into contact with the box
        while (i < overlapColliders.Length)
        {
            // Output all of the collider names
            // Debug.Log("OverlapBox Hit : " + overlapColliders[i].name + " - Count: " + i);
            // Look for Health component in hit collider and call TakeDamage()
            if (overlapColliders[i].TryGetComponent<EnemyHitDetection>(out var target))
            {
                DeliverHitToTarget(target, overlapColliders[i]);
            }
            i++;
        }
    }
#endregion    

#region Helper Functions
    private void DeliverHitToTarget(EnemyHitDetection target, Collider collider)
    {
        // Return if HashSet cannot be added because of duplicated target
        if (!hitThisSwing.Add(target))
            return;

        MoveParticleToHitPoint(collider, target.VFXSwordHit());
        // Enemy take damage
        target.HandleTakingDamage(damage);
        // Enemy react to hit
        target.HandleHitReaction();
    }

    private void MoveParticleToHitPoint(Collider hitCol, ParticleSystem particle)
    {
        // Get closest impact point
        Vector3 hitPosition = hitCol.ClosestPoint(raycastTransform.position);

        // Calculate direction for rotation
        Vector3 direction = (hitPosition - raycastTransform.position).normalized;
        Quaternion faceRotation = Quaternion.LookRotation(direction);

        // Set particle at hit position
        Transform t = particle.transform;
        t.SetPositionAndRotation(hitPosition, faceRotation);
    }

    private void MoveLooping()
    {
        // Move the sword forward using Rigidbody
        Vector3 moveDir = new(0, 0, 1.0f);
        swordRigidbody.MovePosition(swordRigidbody.position + (moveSpeed * Time.fixedDeltaTime * moveDir));

        // Loop back to the started position when exceed certain threshold
        if (swordRigidbody.position.z >= 9.0f)
        {
            Vector3 newPos = new(
                swordRigidbody.position.x, 
                swordRigidbody.position.y, 
                lastZPos);

            swordRigidbody.MovePosition(newPos);
            enabled = false;
        }
    }
#endregion

#region Debugging
    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) 
            return;

        if (detectMethod.Equals(DetectMethod.BoxCastAll))
        {  
            // Check if there has been a hit yet
            // Then draw a ray according to the hit distance
            float castDistance = hitDetect ? rayHit.distance : maxCastDistance;
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
#endregion
}
