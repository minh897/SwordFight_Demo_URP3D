using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Attack data")]
    [SerializeField] private float damage = 10;
    [SerializeField] private float attackCooldown = 0.5f;

    // components
    private PlayerInputHandler inputHandler;
    private PlayerWeaponHitHandler weaponHitHandler;
    private PlayerAttackAnim attackAnim;

    // attack timers
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    public bool IsAttacking {get; private set; }

    void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        weaponHitHandler = GetComponent<PlayerWeaponHitHandler>();
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

        // stop receiving input when attack is performed
        if (IsAttacking) return;

        // perform an attack
        if (inputHandler.InputAttack && attackTimer > nextTimeAttack)
        {
            nextTimeAttack = attackCooldown + Time.time;
            attackAnim.PlayAttackAnim();
        }
    }

    private void HandleAttackStarted()
    {
        IsAttacking = true;
        weaponHitHandler.enabled = true;
        weaponHitHandler.SendDamage(damage);
    }

    private void HandleAttackFinished()
    {
        IsAttacking = false;
        weaponHitHandler.enabled = false;
    }
}
