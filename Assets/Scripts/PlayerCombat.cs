using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{

    // references
    [SerializeField] private GameObject mainWeapon;

    // weapon settings
    [SerializeField] private float attackCooldown = 0.5f;
    // [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private float swingDuration = 0.3f;
    [SerializeField] private float lungeDuration = 0.2f;
    [SerializeField] private float swingYAngle = -180f;
    [SerializeField] private float lungeDistance = 1f;

    private PlayerInputHandler inputHandler;
    private BoxCollider weaponCollider;
    private Coroutine attackAnimationCo;

    private bool isAttacking = false;

    // attack timers
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    private float currentYAngle;
    private float targetYAngle;

    void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        weaponCollider = mainWeapon.GetComponentInChildren<BoxCollider>();

        weaponCollider.enabled = false;
        currentYAngle = mainWeapon.transform.localEulerAngles.y;
        targetYAngle = currentYAngle + swingYAngle;
    }

    void Update()
    {
        attackTimer = Time.time;

        // stop receiving input when attack is performed
        if (isAttacking) return;

        if (inputHandler.AttackInput && attackTimer > nextTimeAttack)
        {
            nextTimeAttack = attackCooldown + Time.time;

            Debug.Log("Player attack");
            PlayAttackAnimation(swingDuration, lungeDuration);
        }
    }

    public bool IsAttacking() => isAttacking;

    private void PlayAttackAnimation(float swingDuration, float lungeDuration)
    {
        // make sure only one coroutine is running
        // prevent one coroutine running multiple times
        if (attackAnimationCo != null) StopCoroutine(attackAnimationCo);
        attackAnimationCo = StartCoroutine(AttackAnimationRoutine(swingDuration, lungeDuration));
    }

    private IEnumerator AttackAnimationRoutine(float swingDuration, float lungeDuration)
    {
        isAttacking = true;
        // enable weapon collider in order to "hit" enemies
        weaponCollider.enabled = true;
         
        StartCoroutine(LungeForwardRoutine(lungeDuration));
        yield return StartCoroutine(WeaponSwingRoutine(swingDuration));

        isAttacking = false;
        // disable weapon collider to prevent "out-of-swing" colliding
        weaponCollider.enabled = false;
    }

    private IEnumerator WeaponSwingRoutine(float duration)
    {
        float elapsed = 0f;
        float startYAngle = currentYAngle;
        float endYAngle = targetYAngle;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // converts time into a normalized progress value
            // and then clamp that progress between 0 and 1 
            // (prevent it to hit 0.2 or 1.1 something like that)
            float t = Mathf.Clamp01(elapsed / duration);
            float yRot = Mathf.Lerp(startYAngle, endYAngle, t);
            Quaternion newRot = Quaternion.Euler(0, yRot, 0);
            mainWeapon.transform.localRotation = newRot;

            yield return null;
        }

        // make sure the rotation hit the target
        mainWeapon.transform.localRotation = Quaternion.Euler(0, endYAngle, 0);
        // swap angle for next swing
        currentYAngle = endYAngle;
        targetYAngle = startYAngle;
    }

    private IEnumerator LungeForwardRoutine(float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward * lungeDistance;
        Vector3 endPos = startPos + direction;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 towardPosition = Vector3.Lerp(startPos, endPos, t);
            transform.position = towardPosition;

            yield return null;
        }

        transform.position = endPos;
    }

}
