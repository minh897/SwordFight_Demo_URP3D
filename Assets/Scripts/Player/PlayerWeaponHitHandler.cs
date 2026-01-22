using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitHandler : MonoBehaviour
{
    [Header("Raycasting")]
    [SerializeField] private float maxCastDistance;
    [SerializeField] private Transform raycastTransform;
    [SerializeField] private LayerMask layerMask;

    [Header("Physics")]
    [SerializeField] private BoxCollider swordCollider;
    [SerializeField] private Rigidbody swordRigidbody;

    private ShakeCamera shakeCamera;
    
    // Only contains unique element
    private HashSet<EnemyHitDetection> hitThisSwing;

    private float weaponDamage;

    public void SendDamage(float damage) => weaponDamage = damage;

    void Awake()
    {
        // playerCombat = GetComponent<PlayerCombat>();
        shakeCamera = GetComponent<ShakeCamera>();
        hitThisSwing = new();
    }

    void OnDisable()
    {
        hitThisSwing.Clear();
    }

    void FixedUpdate()
    {
        DetectHits();
    }

    private void DetectHits()
    {
        // Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), 
        // half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        // Also fetch the hit data
        // int  direction = playerCombat.GetSwingDirection();

        Collider[] hitColliders = Physics.OverlapBox(
            swordCollider.bounds.center,
            raycastTransform.localScale * 0.5f,
            raycastTransform.rotation,
            layerMask);

        int i = 0;
        // Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            // Debug.Log("BoxCastAll Hit : " + hitRays[i].collider.name);
            if (hitColliders[i].TryGetComponent<EnemyHitDetection>(out var target))
            {
                DeliverHitToTarget(target, hitColliders[i]);
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
        target.HandleTakingDamage(weaponDamage);
        // Enemy react to hit
        target.HandleHitReaction(collider, raycastTransform.position);
        // Shake camera
        shakeCamera.PlayBounceShake();
    }
   
}
