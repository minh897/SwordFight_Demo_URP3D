using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject weapon;
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private float attackCooldown = 0.5f;

    private PlayerInputHandler _inputHandler;
    
    // attack timers
    private float attackTimer;
    private float nextTimeAttack;

    void Start()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();

        weapon.SetActive(false);
    }

    void Update()
    {
        attackTimer = Time.time;

        var inputValue = _inputHandler.AttackInput;

        if (!inputValue)
            return;

        if (attackTimer > nextTimeAttack)
        {
            // Do attack logic and play animation
            weapon.SetActive(true);
            Debug.Log("Player attacks!");

            // set cooldown for the next attack
            nextTimeAttack = attackCooldown + Time.time;
        }
        else
        {
            weapon.SetActive(false);
        }
    }

    private IEnumerator AttackAnimRoutine()
    {
        yield return null;
    }
}
