using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitHandler : MonoBehaviour
{
    [Header("Raycasting")]
    [SerializeField] private float maxCastDistance;
    [SerializeField] private Transform raycastTransform;
    [SerializeField] private LayerMask layerMask;

    [Header("Physics")]
    [SerializeField] private BoxCollider weaponCol;
    [SerializeField] private Rigidbody weaponRb;
    
    // Only contains unique element
    private HashSet<EnemyHitDetection> hitThisSwing;

    private float weaponDamage;

    public void SendDamage(float damage) => weaponDamage = damage;

    void Awake()
    {
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
        Collider[] hitColliders = Physics.OverlapBox(
            weaponCol.bounds.center,
            raycastTransform.localScale * 0.5f,
            raycastTransform.rotation,
            layerMask);

        int i = 0;
        // Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].TryGetComponent<EnemyHitDetection>(out var target))
            {
                DeliverHitToTarget(target, hitColliders[i]);
            }
            i++;
        }
    }

    private void DeliverHitToTarget(EnemyHitDetection target, Collider collider)
    {
        // Gameplay rule: a single swing cannot damage the same target twice
        if (!hitThisSwing.Add(target))
            return;

        target.HandleTakingDamage(weaponDamage);
        target.HandleHitReaction(collider, weaponRb.position);
    }
   
}
