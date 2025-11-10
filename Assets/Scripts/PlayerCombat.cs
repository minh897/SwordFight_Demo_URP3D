using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    // references
    [SerializeField] private GameObject mainWeapon;

    // weapon settings
    [SerializeField] private int weaponDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;
    // [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private float swingDuration = 0.3f;
    [SerializeField] private Vector3 swingAngle;
    [SerializeField] private float lungeDuration = 0.2f;
    [SerializeField] private float lungeDistance = 1f;
    [SerializeField] private float overshootYAngle = -30f;
    [SerializeField] private Vector3 stretchScale;

    private PlayerInputHandler inputHandler;
    private BoxCollider weaponCollider;
    private Coroutine attackAnimationCo;

    private bool isAttacking = false;

    // attack timers
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    private float currentYAngle;
    private float targetYAngle;
    private float currentZScale;
    private float targetZScale;

    void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        weaponCollider = mainWeapon.GetComponentInChildren<BoxCollider>();

        weaponCollider.enabled = false;
        
        currentYAngle = mainWeapon.transform.localEulerAngles.y;
        targetYAngle = currentYAngle + swingAngle.z;
        currentZScale = mainWeapon.transform.localScale.z;
        targetZScale = currentZScale + stretchScale.z;
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

    public int GetWeaponDamage() => weaponDamage;

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

        var swingCo = StartCoroutine(WeaponSwingRoutine(swingDuration));
        StartCoroutine(LungeForwardRoutine(lungeDuration));
        StartCoroutine(ScaleWeaponRoutine(swingDuration));
        yield return swingCo;

        isAttacking = false;
        // disable weapon collider to prevent "out-of-swing" colliding
        weaponCollider.enabled = false;
    }

    private IEnumerator WeaponSwingRoutine(float duration)
    {
        float halfDuration = duration * 0.7f; // main swing uses ~70% of total time
        float returnDuration = duration - halfDuration; // remaining time for return

        float elapsed = 0f;
        float startYAngle = currentYAngle;
        float endYAngle = targetYAngle;
        // since both angles are swapped at the end 
        // start and end angle need to be interpolated
        // startYAngle + endYAngle would yield a different result
        float offsetYAngle = Mathf.Lerp(startYAngle, endYAngle, 1f) + overshootYAngle;

        // --- Phase 1: Swing with overshoot ---
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            // converts time into a normalized progress value
            // and then clamp that progress between 0 and 1 
            // (prevent it to hit 0.2 or 1.1 something like that)
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float yRot = Mathf.Lerp(startYAngle, offsetYAngle, t);
            mainWeapon.transform.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        // --- Phase 2: Return from overshoot to end angle ---
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            // Ease back from overshoot to final resting angle
            float yRot = Mathf.Lerp(offsetYAngle, endYAngle, t);
            mainWeapon.transform.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        // make sure the rotation hit the target
        mainWeapon.transform.localRotation = Quaternion.Euler(0, endYAngle, 0);
        // swap angle for next swing
        currentYAngle = endYAngle;
        targetYAngle = startYAngle;
        // change direction after each swing
        overshootYAngle *= -1;
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

    private IEnumerator ScaleWeaponRoutine(float duration)
    {
        float halfDuration = duration * 0.7f; // main swing uses ~70% of total time
        float returnDuration = duration - halfDuration; // remaining time for return

        float elapsed = 0f;
        float startZScale = currentZScale;
        float endZScale = targetZScale;
        Vector3 targetScale = new(1, 1, targetZScale);

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float zScale = Mathf.Lerp(startZScale, endZScale, t);
            mainWeapon.transform.localScale = new(1, 1, zScale);
            yield return null;
        }

        mainWeapon.transform.localScale = targetScale;
        float newStartZScale = endZScale;
        float newTargetZScale = startZScale;
        
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float zScale = Mathf.Lerp(newStartZScale, newTargetZScale, t);
            mainWeapon.transform.localScale = new(1, 1, zScale);
            yield return null;
        }
    }

}
