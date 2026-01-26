using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    public event System.Action OnDead;
    public event System.Action OnImpact;
    public event System.Action<Vector3, Collider> OnCollectHitInfo;

    private Health myHealth;
    private EnemyImpactAnim impactAnim;

    public bool GetHit { get; private set; }

    void Awake()
    {
        myHealth = GetComponent<Health>();
        impactAnim = GetComponent<EnemyImpactAnim>();
    }

    public void HandleHitReaction(Collider hitCollider, Vector3 hitPosition)
    {
        OnImpact?.Invoke();
        OnCollectHitInfo?.Invoke(hitPosition, hitCollider);
        impactAnim.PlayImpactAnim();
    }

    public void HandleTakingDamage(float damage)
    {
        myHealth.TakeDamage(damage);
        if (myHealth.IsDead)
        {
            OnDead?.Invoke();
            Invoke(nameof(DisableEnemyAfter), .3f);
        }
    }

    private void DisableEnemyAfter()
    {
        gameObject.SetActive(false);
    }
}
