using System.Collections;
using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private ParticleSystem vfxImpact;
    [SerializeField] private ParticleSystem vfxExplosion;
    
    [Header("Animation")]
    [SerializeField] private float stunDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float leanDuration;
    [SerializeField] private float pushBackDistance;
    [SerializeField] private Vector3 leanAngle;

    [Header("Audio")]
    [SerializeField] private AudioClip sfxSwordHit;
    [SerializeField] private AudioClip sfxDeath;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField, Range(0, 1)] private float volume;

    // components
    private Health myHealth;
    private SquashAndStretch stretchAnim;
    private EnemyDamageFlash damageFlash;
    private MakeTransparent makeTransparent;
    private AudioSource sfxSource;

    // states
    private bool isStunned = false;

    private float currentAngleX;
    private float targetAngleX;
    private Coroutine getHitRoutine;

    public bool IsStunned() => isStunned;
    public ParticleSystem VFXSwordHit() => vfxImpact;

    void Awake()
    {
        myHealth = GetComponent<Health>();
        stretchAnim = GetComponent<SquashAndStretch>();
        damageFlash = GetComponent<EnemyDamageFlash>();
        makeTransparent = GetComponent<MakeTransparent>();
        sfxSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        currentAngleX = transform.localRotation.x;
        targetAngleX = currentAngleX + leanAngle.x;
    }

    public void HandleHitReaction()
    {
        PlaySFX(sfxSwordHit);
        PlayVFX(vfxImpact);
        PlayStunAnim();
        PlayDamageFlashVFX();
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
            PlayVFX(vfxExplosion);
            // play enemy death sound
            PlaySFX(sfxDeath);
            // disable the enemy GameObject after half a second
            Invoke(nameof(DisableEnemyAfter), .5f);
        }
    }

    private void DisableEnemyAfter()
    {
        gameObject.SetActive(false);
    }

    private void PlaySFX(AudioClip clip)
    {
        AudioManager.Instance.PlaySFX(sfxSource, clip, volume, minPitch, maxPitch);
    }

    private void PlayDamageFlashVFX()
    {
        damageFlash.FlashColor();
    }

    private void PlayVFX(ParticleSystem particle)
    {
        // Activate the particle
        // Make sure the particle and all children are reset
        particle.gameObject.SetActive(true);
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
    }

    private void PlaySquashAndStretchAnim()
    {
        stretchAnim.Play();
    }
    
    private void PlayStunAnim()
    {
        if (getHitRoutine != null) StopCoroutine(getHitRoutine);
        getHitRoutine = StartCoroutine(GetHitRoutine());
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
