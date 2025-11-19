using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyHitDetection : MonoBehaviour
{
    [SerializeField] private float stunDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float pushBackDistance;
    [SerializeField] private float pivotDuration;
    [SerializeField] private Vector3 pivotAngle;

    private Health myWellBeing;

    private bool isStunned = false;
    private Coroutine getHitRoutine;

    private float currentAngleX;
    private float targetAngleX;

    void Start()
    {
        myWellBeing = GetComponent<Health>();

        currentAngleX = transform.localRotation.x;
        targetAngleX = currentAngleX + pivotAngle.x;
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

        // var pushRoutine = StartCoroutine(GetPushBackRoutine(pushDuration, direction));
        yield return StartCoroutine(GetPivotRoutine(pivotDuration));

        isStunned = false;
    }

    private IEnumerator GetPushBackRoutine(float duration)
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

    private IEnumerator GetPivotRoutine(float duration)
    {
        float elapsed = 0f;
        float startAngleX = currentAngleX;
        float endAngleX = targetAngleX;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newAngleX = Mathf.LerpAngle(startAngleX, endAngleX, t);
            transform.rotation = Quaternion.Euler(newAngleX, transform.eulerAngles.y, 0);
            yield return null;
        }

    }
}
