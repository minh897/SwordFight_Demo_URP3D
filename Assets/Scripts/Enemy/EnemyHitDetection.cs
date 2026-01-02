using System.Collections;
using UnityEngine;

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
    private SquashAndStretch stretchController;
    private EnemyDamageFlash flashController;
    private MakeTransparent makeTransparent;

    // private fields
    private bool isStunned = false;
    private float currentAngleX;
    private float targetAngleX;
    private Coroutine getHitRoutine;

    void Start()
    {
        myWellBeing = GetComponent<Health>();
        stretchController = GetComponent<SquashAndStretch>();
        flashController = GetComponent<EnemyDamageFlash>();
        makeTransparent = GetComponent<MakeTransparent>();

        currentAngleX = transform.localRotation.x;
        targetAngleX = currentAngleX + leanAngle.x;
    }

    void OnTriggerEnter(Collider other)
    {
        // check if the colliding object has "Weapon" tag
        if (other.CompareTag("Weapon"))
        {
            PlayImpactSFX();
            PlayGetHitVFX();
            TakeDamageFrom(other);

            // if the enemy is dead taking damage
            if (myWellBeing.IsDead)
            {
                Debug.Log("Enemy is dead");
                // turn the enemy transparent to simulate death
                makeTransparent.SetMatToTransparent(flashController.Renderers);
                // disable the enemy GameObject after half a second
                Invoke(nameof(DisableEnemyAfter), .5f);
            }
        }
    }

    public bool IsStunned() => isStunned;

    private void DisableEnemyAfter()
    {
        gameObject.SetActive(false);
    }

    // REFACTOR
    private void TakeDamageFrom(Collider other)
    {
        // get weapon damage from PlayerCombat
        int damage = other.gameObject.
            GetComponentInParent<PlayerCombat>().
            GetWeaponDamage();
        // receive damage through IDamageable
        myWellBeing.TakeDamage(damage);
    }

    private void PlayGetHitVFX()
    {
        if (getHitRoutine != null) StopCoroutine(getHitRoutine);
        getHitRoutine = StartCoroutine(GetHitRoutine());
    }

    private void PlayImpactSFX()
    {
        SoundFXManager.instance.PlaySoundFXClip(sfxImpact, transform, volume, minPitch, maxPitch);
    }

    private void PlayDamageFlashVFX()
    {
        flashController.FlashColor();
    }

    private void PlaySquashAndStretchVFX()
    {
        stretchController.Play();
    }

    private IEnumerator GetHitRoutine()
    {
        isStunned = true;

        PlayDamageFlashVFX();
        PlaySquashAndStretchVFX();

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
