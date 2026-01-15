using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordHitHandler : MonoBehaviour
{
    [Header("Raycasting")]
    [SerializeField] private float maxCastDistance;
    [SerializeField] private Transform raycastTransform;
    [SerializeField] private LayerMask layerMask;

    [Header("Physics")]
    [SerializeField] private BoxCollider swordCollider;
    [SerializeField] private Rigidbody swordRigidbody;

    // Only contains unique element
    private HashSet<EnemyHitDetection> hitThisSwing;
    private PlayerCombat playerCombat;
    private ShakeCamera shakeCamera;

    void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        shakeCamera = GetComponent<ShakeCamera>();
        hitThisSwing = new();
    }

    void OnDisable()
    {
        hitThisSwing.Clear();
    }

    void FixedUpdate()
    {
        if (playerCombat.IsAttacking)
            DetectHits();
    }

    private void DetectHits()
    {
        // Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), 
        // half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        // Also fetch the hit data
        // int  direction = playerCombat.GetSwingDirection();

        RaycastHit[] hitRays = Physics.BoxCastAll(
            swordCollider.bounds.center,
            raycastTransform.localScale * 0.5f,
            raycastTransform.right * 1,
            raycastTransform.rotation,
            maxCastDistance, 
            layerMask);

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

    private void DeliverHitToTarget(EnemyHitDetection target, Collider collider)
    {
        // Return if HashSet cannot be added because of duplicated target
        if (!hitThisSwing.Add(target))
            return;

        // Enemy take damage
        float damage = playerCombat.GetWeaponDamage();
        target.HandleTakingDamage(damage);
        // Enemy react to hit
        target.HandleHitReaction(collider, raycastTransform.position);
        // Shake camera
        shakeCamera.PlayBounceShake();
    }
   
}
