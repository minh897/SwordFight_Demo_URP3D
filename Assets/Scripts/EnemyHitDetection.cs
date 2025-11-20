using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyHitDetection : MonoBehaviour
{
    [SerializeField] private float stunDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float leanDuration;
    [SerializeField] private float shrinkDuration;
    [SerializeField] private float pushBackDistance;
    [SerializeField] private Vector3 leanAngle;
    [SerializeField] private Vector3 shrinkScale;

    private Health myWellBeing;

    private bool isStunned = false;
    private Coroutine getHitRoutine;

    private float currentAngleX;
    private float targetAngleX;
    private Vector3 currentScale;
    private Vector3 targetScale;

    void Start()
    {
        myWellBeing = GetComponent<Health>();

        currentAngleX = transform.localRotation.x;
        targetAngleX = currentAngleX + leanAngle.x;
        currentScale = transform.localScale;
        targetScale = currentScale + shrinkScale;
    }

    void OnTriggerEnter(Collider other)
    {
        // check if the colliding object has "Weapon" tag
        if (other.CompareTag("Weapon"))
        {
            GetHitWith(other.gameObject);
        }
    }

    public bool IsStunned() => isStunned;

    private void GetHitWith(GameObject weapon)
    {
        if (getHitRoutine != null) StopCoroutine(getHitRoutine);
        getHitRoutine = StartCoroutine(GetHitRoutine(weapon));
    }

    private IEnumerator GetHitRoutine(GameObject weapon)
    {
        // start a countdown for stun durtation
        isStunned = true;
        // get weapon damage from PlayerCombat
        int damage = weapon.GetComponentInParent<PlayerCombat>().GetWeaponDamage();
        // receive damage through IDamageable
        myWellBeing.TakeDamage(damage);

        var pushRoutine = StartCoroutine(PushBackRoutine(pushDuration));
        StartCoroutine(LeanBackwardRoutine(leanDuration));
        StartCoroutine(ShrinkRoutine(shrinkDuration));
        yield return pushRoutine;

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
        float startAngleX = currentAngleX;
        float endAngleX = targetAngleX;
        float halfDuration = duration * 0.5f;
        float returnDuration = duration - halfDuration;

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

    private IEnumerator ShrinkRoutine(float duration)
    {
        float elapsed = 0f;
        float initialDuration = duration * 0.5f;
        float returnDuration = duration - initialDuration;
        Vector3 startScale = currentScale;
        Vector3 endScale = targetScale;

        // phase 1: interpolate from default to target
        while (elapsed < initialDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / initialDuration);
            var result = Vector3.Lerp(startScale, endScale, t);
            transform.localScale = result;
            yield return null;
        }

        // phase2: return from target back to default
        elapsed = 0;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / initialDuration);
            var result = Vector3.Lerp(endScale, currentScale, t);
            transform.localScale = result;
            yield return null;
        }
    }
}
