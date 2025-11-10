using UnityEngine;

[RequireComponent(typeof(Health))]
public class HitDetection : MonoBehaviour
{
    private Health myWellBeing;

    void Start()
    {
        myWellBeing = GetComponent<Health>();
    }

    void OnTriggerEnter(Collider other)
    {
        // check if the colliding object is a weapon
        if (other.CompareTag("Weapon"))
        {
            // get weapon damage from PlayerCombat
            int damage = other.GetComponentInParent<PlayerCombat>().GetWeaponDamage();
            // receive damage through IDamageable
            myWellBeing.TakeDamage(damage);
        }
    }
}
