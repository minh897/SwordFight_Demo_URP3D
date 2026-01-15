using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private float swordDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem vfxSwordSlash;

    [Header("Audio")]
    [SerializeField] private AudioClip sfxSwordSwing;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField, Range(0, 1)] private float volume;

    // components
    private AudioSource sfxSource;
    private PlayerInputHandler inputHandler;
    private PlayerSwordHitHandler swordHitHandler;
    private PlayerAnimAttack playerAnim;

    private Coroutine attackAnimationCo;
    private int lastSwingDirection = 1; // 1 = right, -1 = left
    private int currentSwingDirection;

    // attack timers
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    public bool IsAttacking {get; private set; }

    public float GetWeaponDamage() => swordDamage;

    void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
        inputHandler = GetComponent<PlayerInputHandler>();
        swordHitHandler = GetComponent<PlayerSwordHitHandler>();
        playerAnim = GetComponent<PlayerAnimAttack>();

        currentSwingDirection = lastSwingDirection;
    }

    void Update()
    {
        // reset attacking state when cooldown is up
        if (IsAttacking && attackTimer >= nextTimeAttack)
        {
            // flip swing direction
            currentSwingDirection *= -1;
            IsAttacking = false;
            swordHitHandler.enabled = false;
        }
    }

    void FixedUpdate()
    {
        attackTimer = Time.time;

        // stop receiving input when attack is performed
        if (IsAttacking) return;

        // perform an attack
        if (inputHandler.InputAttack && attackTimer > nextTimeAttack)
        {
            IsAttacking = true;
            swordHitHandler.enabled = true;
            nextTimeAttack = attackCooldown + Time.time;

            // Play attack animation
            PlayAttackAnimation();
            // Play sound effect
            PlaySwordSwingSFX();
            // Play visual effect
            PlaySwordSlashVFX();
        }
    }

    private void PlayAttackAnimation()
    {
        // make sure only one coroutine is running
        // prevent one coroutine running multiple times
        if (attackAnimationCo != null) StopCoroutine(attackAnimationCo);
        attackAnimationCo = StartCoroutine(playerAnim.AttackAnimationRoutine());
    }

    public void PlaySwordSwingSFX()
    {
        AudioManager.Instance.PlaySFX(sfxSwordSwing, sfxSource, volume, minPitch, maxPitch);
    }

    public void PlaySwordSlashVFX()
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
