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
        // visualEffect.MoveVFXToColliderPos(visualEffect.VFXSwordHit(), collider, position);
        // visualEffect.PlayImpactVFX();
        // damageFlash.FlashColor();
        // soundEffect.PlaySwordHitSFX();
        // impactAnim.PlayImpactAnim();
    }

    public void HandleTakingDamage(float damage)
    {
        myHealth.TakeDamage(damage);

        // handle enemy dead state
        if (myHealth.IsDead)
        {
            // visualEffect.PlayExplosionVFX();
            // soundEffect.PlayDeathSFX();
            makeTransparent.SetMatToTransparent(damageFlash.Renderers);
            Invoke(nameof(DisableEnemyAfter), .5f);
        }
    }

    private void DisableEnemyAfter()
    {
        gameObject.SetActive(false);
    }
}
