using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Attack data")]
    [SerializeField] private float damage = 10;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Visual effects")]
    [SerializeField] private ParticleSystem vfxSwordSlash;

    [Header("Audio data")]
    [SerializeField] private AudioClip sfxSwordSwing;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField, Range(0, 1)] private float volume;

    // components
    private AudioSource sfxSource;
    private PlayerInputHandler inputHandler;
    private PlayerSwordHitHandler swordHitHandler;
    private PlayerAnimAttack attackAnim;

    private int lastSwingDirection = 1; // 1 = right, -1 = left
    private int currentSwingDirection;

    // attack timers
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    public bool IsAttacking {get; private set; }
    public event System.Action OnAttacking;

    void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
        inputHandler = GetComponent<PlayerInputHandler>();
        swordHitHandler = GetComponent<PlayerSwordHitHandler>();
        attackAnim = GetComponent<PlayerAnimAttack>();

        currentSwingDirection = lastSwingDirection;
    }

    void OnEnable()
    {
        attackAnim.OnStarted += HandleAttackStarted;
        attackAnim.OnFinished += HandleAttackFinished;
    }

    void OnDisable()
    {
        attackAnim.OnStarted -= HandleAttackStarted;
        attackAnim.OnFinished -= HandleAttackFinished;
    }

    void FixedUpdate()
    {
        attackTimer = Time.time;

        // stop receiving input when attack is performed
        if (IsAttacking) return;

        // perform an attack
        if (inputHandler.InputAttack && attackTimer > nextTimeAttack)
        {
            OnAttacking?.Invoke();
            nextTimeAttack = attackCooldown + Time.time;

            // Play attack animation
            attackAnim.PlayAttackAnim();
            // Play sound effect
            PlaySwordSwingSFX();
            // Play visual effect
            PlaySwordSlashVFX();
        }
    }

    private void HandleAttackStarted()
    {
        IsAttacking = true;
        swordHitHandler.enabled = true;
        swordHitHandler.SendDamage(damage);
    }

    private void HandleAttackFinished()
    {
        IsAttacking = false;
        swordHitHandler.enabled = false;
        currentSwingDirection *= -1;
    }

    private void PlaySwordSwingSFX()
    {
        AudioManager.Instance.PlaySFX(sfxSwordSwing, sfxSource, volume, minPitch, maxPitch);
    }

    private void PlaySwordSlashVFX()
    {
        vfxSwordSlash.Play();

        // if swing angle has changed
        if (currentSwingDirection != lastSwingDirection)
        {
            // flip the swordslash direction
            vfxSwordSlash.transform.localRotation =
                currentSwingDirection == 1 ? Quaternion.identity : Quaternion.Euler(0, 0, 180);
            lastSwingDirection = currentSwingDirection;
        }
    }
}
