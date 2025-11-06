using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{

    // references
    [SerializeField] private GameObject mainWeapon;

    // settings
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float swingDuration = 0.1f;
    [SerializeField] private float lungeDuration = 0.5f;
    [SerializeField] private float weaponSwingAngle = 180f;

    private PlayerInputHandler inputHandler;
    private BoxCollider weaponCollider;
    private Coroutine weaponSwingRoutine;
    private Coroutine lungeForwadRoutine;

    public bool isAttacking = false;
    
    // attack timers
    private float attackTimer = 0f;
    private float nextTimeAttack = 0f;

    private float currentYRotation;
    private float targetYRotation;


    void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        weaponCollider = mainWeapon.GetComponentInChildren<BoxCollider>();

        weaponCollider.enabled = false;
        currentYRotation = mainWeapon.transform.localEulerAngles.y;
        targetYRotation = currentYRotation + weaponSwingAngle;
    }

    void Update()
    {
        attackTimer = Time.time;

        if (inputHandler.AttackInput && attackTimer > nextTimeAttack)
        {
            isAttacking = true;
            weaponCollider.enabled = true;
            nextTimeAttack = attackCooldown + Time.time;
            Debug.Log("Player attack");

            // disable movement input using PlayerInputHandler
            inputHandler.GetMoveAction().Disable();
            // play attack animation coroutine
            SwingWeapon();
            // play lunge forward animation coroutine
            LungeForward();
        }

        if (!isAttacking)
        {
            // disable collider to prevent out-of-swing detection
            weaponCollider.enabled = false;
            // enable movement input back when both animation is done
            inputHandler.GetMoveAction().Enable();
        }
    }

    public void SwingWeapon()
    {
        if (weaponSwingRoutine != null)
            weaponSwingRoutine = null;

        mainWeapon.GetComponent<Rigidbody>();
        weaponSwingRoutine = StartCoroutine(WeaponSwingRoutine());
    }

    public void LungeForward()
    {
        if (lungeForwadRoutine != null)
            lungeForwadRoutine = null;

        lungeForwadRoutine = StartCoroutine(LungeForwardRoutine());
    }

    private IEnumerator WeaponSwingRoutine()
    {
        float elapsed = 0f;
        float startY = currentYRotation;
        float endY = targetYRotation;

        Quaternion targetRot = Quaternion.Euler(0, endY, 0);

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;

            // converts time into a normalized progress value
            // and then clamp that progress between 0 and 1 
            // (prevent that value to hit 0.2 or 1.1 something like that)
            float t = Mathf.Clamp01(elapsed / swingDuration);
            float yRot = Mathf.Lerp(startY, endY, t);
            Quaternion newRot = Quaternion.Euler(0, yRot, 0);
            mainWeapon.transform.localRotation = newRot;

            yield return null;
        }

        // make sure the rotation hit the target
        mainWeapon.transform.localRotation = targetRot;
        // swap angle for next swing
        currentYRotation = endY;
        targetYRotation = startY;
    }

    private IEnumerator LungeForwardRoutine()
    {
        float elapsed = 0f;
        float lungDistance = 1f;
        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward * lungDistance;
        Vector3 endPos = transform.position + direction;
        
        while (elapsed < lungeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.5f;
            var towardPosition = Vector3.Lerp(startPos, endPos, t);
            transform.position = towardPosition;

            // wait for the swing to finish
            yield return weaponSwingRoutine;
        }

        isAttacking = false;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var lungDistance = 1f;
        var direction = transform.forward * lungDistance;
        var targetPosition = transform.position + direction;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
