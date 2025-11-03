using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField] private int weaponDamage;

    void OnTriggerEnter(Collider other)
    {
        // because other collider is a child object
        // we need its parent transform in order to get
        // the IDamageable interface 
        var otherParent = other.transform.parent;

        if (otherParent.TryGetComponent(out IDamageable damageableObject))
        {
            Debug.Log("Hit target OnTriggerEnter");
            damageableObject.TakeDamage(weaponDamage);
        }
    }
}
