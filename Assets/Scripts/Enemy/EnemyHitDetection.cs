using System.Collections;
using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float stunDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float leanDuration;
    [SerializeField] private float pushBackDistance;
    [SerializeField] private Vector3 leanAngle;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem vfxSwordHit;

    [Header("Audio")]
    [SerializeField] private AudioClip sfxSwordHit;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField, Range(0, 1)] private float volume;

    // components
    private Health myHealth;
    private CapsuleCollider myCollider;
    private SquashAndStretch stretchAnim;
    private EnemyDamageFlash damageFlash;
    private MakeTransparent makeTransparent;

    // states
    private bool isStunned = false;

    private float currentAngleX;
    private float targetAngleX;
    private Coroutine getHitRoutine;

    public bool IsStunned() => isStunned;

    void Awake()
    {
        myHealth = GetComponent<Health>();
        myCollider = GetComponent<CapsuleCollider>();
        stretchAnim = GetComponent<SquashAndStretch>();
        damageFlash = GetComponent<EnemyDamageFlash>();
        makeTransparent = GetComponent<MakeTransparent>();
    }

    void Start()
    {
        currentAngleX = transform.localRotation.x;
        targetAngleX = currentAngleX + leanAngle.x;
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     // check if the colliding object has "Weapon" tag
    //     if (!other.CompareTag("Weapon"))
    //         return;
            
    //     PlayStunAnim();
    //     PlayImpactSFX();
    //     PlayImpactVFX(other);
    //     PlayDamageFlashVFX();
    //     TakeDamageFrom(other);
    //     CheckDeadState();
    // }

#region Refactor
    private void TakeDamageFrom(Collider other)
    {
        // get weapon damage from PlayerCombat
        int damage = other.gameObject.
            GetComponentInParent<PlayerCombat>().
            GetWeaponDamage();
        // receive damage through IDamageable
        myHealth.TakeDamage(damage);
    }

    private void CheckDeadState()
    {
        // if the enemy is dead taking damage
        if (myHealth.IsDead)
        {
            Debug.Log("Enemy is dead");
            // turn the enemy transparent to simulate death
            makeTransparent.SetMatToTransparent(damageFlash.Renderers);
            // disable the enemy GameObject after half a second
            Invoke(nameof(DisableEnemyAfter), .5f);
        }
    }
#endregion

    private void DisableEnemyAfter()
    {
        gameObject.SetActive(false);
    }
    
    private void PlayStunAnim()
    {
        if (getHitRoutine != null) StopCoroutine(getHitRoutine);
        getHitRoutine = StartCoroutine(GetHitRoutine());
    }

    private void PlayImpactSFX()
    {
        SoundFXManager.instance.PlaySoundFXClip(sfxSwordHit, transform, volume, minPitch, maxPitch);
    }

    private void PlayDamageFlashVFX()
    {
        damageFlash.FlashColor();
    }

    private void PlayImpactVFX(Collider weaponCollider)
    {
        // Get closest impact point
        Transform weaponTransform = weaponCollider.transform;
        Vector3 hitPosition = myCollider.ClosestPoint(weaponTransform.position);

        // Calculate direction for rotation
        Vector3 direction = (hitPosition - weaponTransform.position).normalized;
        Quaternion faceRotation = Quaternion.LookRotation(direction);

        // Set particle at hit position
        Transform t = vfxSwordHit.transform;
        t.SetPositionAndRotation(hitPosition, faceRotation);

        // Activate the particle
        // Make sure the particle and all children are reset
        vfxSwordHit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        vfxSwordHit.Play();
    }

    private void PlaySquashAndStretchAnim()
    {
        stretchAnim.Play();
    }

    private IEnumerator GetHitRoutine()
    {
        isStunned = true;

        PlaySquashAndStretchAnim();
        Coroutine waitForRoutine = StartCoroutine(PushBackRoutine(pushDuration));
        StartCoroutine(LeanBackwardRoutine(leanDuration));
        yield return waitForRoutine;

        isStunned = false;
    }

    private IEnumerator PushBackRoutine(float duration)
    {
        float elapsed = 0f;
        // get enemy direction
        Vector3 direction = gameObject.GetComponent<EnemyFollow>().GetMovingDirection();
        // flip the current direction to get the opposite direction
        Vector3 oppositeDir = -1 * direction;
        // get enemy current position
        Vector3 startPos = transform.position;
        // calculate the target position
        Vector3 endPos = startPos + oppositeDir * pushBackDistance;

        // interpolate between current position and target position
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 nextPos = Vector3.Lerp(startPos, endPos, t);
            transform.position = nextPos;
            yield return null;
        }

        transform.position = endPos;
    }

    private IEnumerator LeanBackwardRoutine(float duration)
    {
        float elapsed = 0f;
        float halfDuration = duration * 0.5f;
        float returnDuration = duration - halfDuration;
        float startAngleX = currentAngleX;
        float endAngleX = targetAngleX;

        // phase 1: pivot backward
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float newAngleX = Mathf.LerpAngle(startAngleX, endAngleX, t);
            transform.rotation = Quaternion.Euler(newAngleX, transform.eulerAngles.y, 0);
            yield return null;
        }

        // phase2: return to normal rotation
        elapsed = 0;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float newAngleX = Mathf.LerpAngle(endAngleX, startAngleX, t);
            transform.rotation = Quaternion.Euler(newAngleX, transform.eulerAngles.y, 0);
            yield return null;
        }
    }

}
