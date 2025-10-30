using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    // references
    [SerializeField] private GameObject weaponDefaultPrefab;
    [SerializeField] private Transform handLeft;
    [SerializeField] private Transform handRight;
    [SerializeField] private Transform pivotPoint;

    // settings
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float defaultSwingDuration = 0.1f;

    private PlayerInputHandler _inputHandler;
    private GameObject equipedWeapon;
    private Coroutine attackRoutine;
    
    // attack timers
    private float attackTimer;
    private float nextTimeAttack;

    void Start()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();

        // handLeft.gameObject.SetActive(false);
        // handRight.gameObject.SetActive(false);

        EquipWeaponDefault(weaponDefaultPrefab, handRight);
    }

    private void EquipWeaponDefault(GameObject weaponPrefab, Transform equipHand)
    {
        equipedWeapon = Instantiate(weaponPrefab, transform, false);
        equipedWeapon.transform.position = equipHand.position;
        equipedWeapon.transform.forward = equipHand.right;
    }

    void Update()
    {
        attackTimer = Time.time;

        var inputValue = _inputHandler.AttackInput;

        if (inputValue && attackTimer > nextTimeAttack)
        {
            // Do attack logic and play animation
            Debug.Log("Player attacks!");
            PlayWeaponSwing(attackCooldown);

            // set cooldown for the next attack
            nextTimeAttack = attackCooldown + Time.time;
        }

        // if (attackTimer > nextTimeAttack)
        // {
        //     // Do attack logic and play animation
        //     Debug.Log("Player attacks!");

        //     // set cooldown for the next attack
        //     nextTimeAttack = attackCooldown + Time.time;
        // }
    }

    private void PlayWeaponSwing(float? duration = null)
    {
        if (attackRoutine != null) attackRoutine = null;
        attackRoutine = StartCoroutine(WeaponSwingRoutine(duration));
    }

    private IEnumerator WeaponSwingRoutine(float? newDuration = null)
    {
        float time = 0;
        float duration = newDuration ?? defaultSwingDuration;

        while (time < duration)
        {
            equipedWeapon.transform.rotation = Quaternion.Lerp(
                handRight.rotation, handLeft.rotation, time / duration);

            time += Time.deltaTime;
            yield return null;
        }
        
        equipedWeapon.transform.rotation = handLeft.rotation;
    }
}
