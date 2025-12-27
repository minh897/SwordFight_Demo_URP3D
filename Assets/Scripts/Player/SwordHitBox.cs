using UnityEngine;
using CameraShake;

public class SwordHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] BounceShake.Params bounceShakeParams;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            SpawnHitVFX(other);
            CameraShaker.Shake(new BounceShake(bounceShakeParams));
        }
    }

    private void SpawnHitVFX(Collider other)
    {
        // Get closest impact point
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        // Calculate direction for rotation
        Vector3 direction = (hitPoint - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        // Create vfx at hit position
        Instantiate(hitVFX, hitPoint, rotation, other.transform.parent);
    }
}