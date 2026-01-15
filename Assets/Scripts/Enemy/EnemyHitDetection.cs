using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    // components
    private Health myHealth;
    private MakeTransparent makeTransparent;
    private EnemyDamageFlash damageFlash;
    private EnemyImpactAnim impactAnim;
    private EnemyVisualEffect visualEffect;
    private EnemySoundEffect soundEffect;

    public bool GetHit { get; private set; }

    void Awake()
    {
        myHealth = GetComponent<Health>();
        makeTransparent = GetComponent<MakeTransparent>();
        damageFlash = GetComponent<EnemyDamageFlash>();
        impactAnim = GetComponent<EnemyImpactAnim>();
        visualEffect = GetComponent<EnemyVisualEffect>();
        soundEffect = GetComponent<EnemySoundEffect>();
    }

    public void HandleHitReaction(Collider collider, Vector3 position)
    {
        // Move impact visual effect to hit position
        var particle = visualEffect.VFXSwordHit();
        visualEffect.MoveVFXToColliderPos(particle, collider, position);
        // Play impact visual effect
        visualEffect.PlayImpactVFX();
        // Play damage flash
        damageFlash.FlashColor();
        // Play sword hit sound effect
        soundEffect.PlaySwordHitSFX();
        // Play stun animation
        impactAnim.PlayImpactAnim();
    }

    public void HandleTakingDamage(float damage)
    {
        myHealth.TakeDamage(damage);

        // handle enemy dead state
        if (myHealth.IsDead)
        {
            // turn the enemy transparent to simulate death
            makeTransparent.SetMatToTransparent(damageFlash.Renderers);
            // play enemy explosion vfx
            visualEffect.PlayExplosionVFX();
            // play enemy death sound
            soundEffect.PlayDeathSFX();
            // disable the enemy GameObject after half a second
            Invoke(nameof(DisableEnemyAfter), .5f);
        }
    }

    private void DisableEnemyAfter()
    {
        gameObject.SetActive(false);
    }
}
