using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField] private int weaponDamage;

    void OnTriggerEnter(Collider other)
    {
        // because the collider is within the child object
        // we need its parent transform in order to get component
        // implemented the IDamageable interface 
        var otherParent = other.transform.parent.gameObject;

        if (otherParent.TryGetComponent(out IDamageable damageableObject))
        {
            Debug.Log("Hit target OnTriggerEnter");
            damageableObject.TakeDamage(weaponDamage);
        }
    }
}
