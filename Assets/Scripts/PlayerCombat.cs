using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    // references
    [SerializeField] private GameObject _mainWeapon;

    // settings
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float swingDuration = 0.1f;
    [SerializeField] private float weaponSwingAngle = 180f;

    private PlayerInputHandler _inputHandler;
    private Coroutine weaponSwingRoutine;
    private BoxCollider weaponCollider;

    public bool isAttacking = false;
    
    // attack timers
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    private float currentYRotation;
    private float targetYRotation;


    void Start()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        weaponCollider = _mainWeapon.GetComponentInChildren<BoxCollider>();

        weaponCollider.enabled = false;
        currentYRotation = _mainWeapon.transform.localEulerAngles.y;
        targetYRotation = currentYRotation + weaponSwingAngle;
    }

    void Update()
    {
        attackTimer = Time.time;

        if (_inputHandler.AttackInput && attackTimer > nextTimeAttack)
        {
            isAttacking = true;
            weaponCollider.enabled = true;
        }

        if (isAttacking)
        {
            isAttacking = false;
            nextTimeAttack = attackCooldown + Time.time;

            Debug.Log("Player attack");
            // player attacking animation
            SwingWeapon(_mainWeapon.transform);
        }
    }

    public void SwingWeapon(Transform weaponTransform)
    {
        if (weaponSwingRoutine != null)
            weaponSwingRoutine = null;

        weaponSwingRoutine = StartCoroutine(WeaponSwingRoutine(weaponTransform));
    }

    private IEnumerator WeaponSwingRoutine(Transform weapon)
    {
        float elapsed = 0f;
        float startY = currentYRotation;
        float endY = targetYRotation;
        Quaternion targetRot = Quaternion.Euler(0, targetYRotation, 0);

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;

            // converts time into a normalized progress value
            // and then clamp that progress between 0 and 1 
            // (prevent that value to hit 0.2 or 1.1 something like that)
            float t = Mathf.Clamp01(elapsed / swingDuration);
            float yRot = Mathf.Lerp(startY, endY, t);
            weapon.localRotation = Quaternion.Euler(0, yRot, 0);

            yield return null;
        }
        // make sure the rotation hit the target
        weapon.localRotation = targetRot;
        // swap angle for next swing
        currentYRotation = endY;
        targetYRotation = startY;
        // disable collider to prevent out-of-swing detection
        weaponCollider.enabled = false;
    }
}
