using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SquashAndStretch))]
[RequireComponent(typeof(EnemyHitFlash))]
public class EnemyHitDetection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioClip sfxImpact;

    [SerializeField] private float stunDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float leanDuration;
    [SerializeField] private float pushBackDistance;
    [SerializeField] private Vector3 leanAngle;

    [Header("Sound Setting")]
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField, Range(0, 1)] private float volume;

    // components
    private Health myWellBeing;
    private SquashAndStretch anim_SquashAndStretch;
    private EnemyHitFlash hitFlash;

    private bool isStunned = false;
    private float currentAngleX;
    private float targetAngleX;
    private Coroutine getHitRoutine;

    void Start()
    {
        myWellBeing = GetComponent<Health>();
        anim_SquashAndStretch = GetComponent<SquashAndStretch>();
        hitFlash = GetComponent<EnemyHitFlash>();

        currentAngleX = transform.localRotation.x;
        targetAngleX = currentAngleX + leanAngle.x;
    }

    void OnTriggerEnter(Collider other)
    {
        // check if the colliding object has "Weapon" tag
        if (other.CompareTag("Weapon"))
        {
            PlayGetHit();
            PlaySFXImpact();
            PlayHitFlash();
            TakeDamageFrom(other);
        }
    }

    public bool IsStunned() => isStunned;

    private void PlayHitFlash()
    {
        hitFlash.PlayHitFlash();
    }

    private void TakeDamageFrom(Collider other)
    {
        // get weapon damage from PlayerCombat
        int damage = other.gameObject.
            GetComponentInParent<PlayerCombat>().
            GetWeaponDamage();
        // receive damage through IDamageable
        myWellBeing.TakeDamage(damage);
    }

    private void PlaySFXImpact()
    {
        SoundFXManager.instance.PlaySoundFXClip(sfxImpact, transform, volume, minPitch, maxPitch);
    }

    private void PlayGetHit()
    {
        if (getHitRoutine != null) StopCoroutine(getHitRoutine);
        getHitRoutine = StartCoroutine(GetHitRoutine());
    }

    private IEnumerator GetHitRoutine()
    {
        // start a countdown for stun durtation
        isStunned = true;

        anim_SquashAndStretch.Play();
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
