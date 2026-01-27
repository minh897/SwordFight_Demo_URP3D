using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    public bool IsAttacking {get; private set; }

    [Header("Attack data")]
    [SerializeField] private float damage = 10;
    [SerializeField] private float attackCooldown = 0.5f;
    
    [Header("Raycasting")]
    [SerializeField] private Transform raycastTransform;
    [SerializeField] private Collider raycastCollider;
    [SerializeField] private LayerMask layerMask;

    private PlayerInputHandler inputHandler;
    private PlayerAttackAnim attackAnim;
    
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    private HashSet<EnemyHitDetection> hitThisSwing = new();

    void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        attackAnim = GetComponent<PlayerAttackAnim>();
    }

    void OnEnable()
    {
        attackAnim.OnAttackStarted += HandleAttackStarted;
        attackAnim.OnAttackFinished += HandleAttackFinished;
    }

    void OnDisable()
    {
        attackAnim.OnAttackStarted -= HandleAttackStarted;
        attackAnim.OnAttackFinished -= HandleAttackFinished;
    }

    void FixedUpdate()
    {
        attackTimer = Time.time;

        // Return to prevent multiple attack inputs
        if (IsAttacking)
        {
            DetectHits();
            return;
        }

        if (attackTimer > nextTimeAttack && inputHandler.InputAttack)
        {
            nextTimeAttack = attackCooldown + Time.time;
            attackAnim.PlayAttackAnim();
        }
    }

    private void HandleAttackStarted()
    {
        IsAttacking = true;
    }

    private void HandleAttackFinished()
    {
        IsAttacking = false;
        hitThisSwing.Clear();
    }

    private void DetectHits()
    {
        Collider[] hitColliders = Physics.OverlapBox(
            raycastCollider.bounds.center,
            raycastTransform.localScale * 0.5f,
            raycastTransform.rotation,
            layerMask);

        int i = 0;
        // Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].TryGetComponent<EnemyHitDetection>(out var target))
            {
                // Gameplay rule: a single swing cannot damage the same target twice
                if (!hitThisSwing.Add(target))
                    return;

                target.HandleTakingDamage(damage);
                target.HandleHitReaction(hitColliders[i], raycastTransform.position);
            }
            i++;
        }
    }
}
