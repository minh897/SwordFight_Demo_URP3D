using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyHitDetection : MonoBehaviour
{
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private float pushBackDistance = 1f;

    private Health myWellBeing;

    private bool isStunned = false;
    private int weaponDamage;
    private Coroutine getHitRoutine;

    void Start()
    {
        myWellBeing = GetComponent<Health>();
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
        // get enemy direction
        Vector3 direction = gameObject.GetComponent<EnemyFollow>().GetMovingDirection();

        yield return null;

        isStunned = false;
    }

    private IEnumerator GetPushBackRoutine(float duration, Vector3 direction)
    {
        float elapsed = 0f;
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
}
