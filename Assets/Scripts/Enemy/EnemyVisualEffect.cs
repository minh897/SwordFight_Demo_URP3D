using UnityEngine;

public class EnemyVisualEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfxImpact;
    [SerializeField] private ParticleSystem vfxExplosion;

    public ParticleSystem VFXSwordHit() => vfxImpact;
    
    public void PlayImpactVFX()
    {
        PlayVFX(vfxImpact);
    }

    public void PlayExplosionVFX()
    {
        PlayVFX(vfxExplosion);
    }

    public void MoveVFXToColliderPos(ParticleSystem particle, Collider collider, Vector3 position)
    {
        // Get closest impact point
        Vector3 hitPosition = collider.ClosestPoint(position);

        // Calculate direction for rotation
        Vector3 direction = (hitPosition - position).normalized;
        if (!direction.Equals(Vector3.zero))
        {
            Quaternion faceRotation = Quaternion.LookRotation(direction);    
            // Set particle at hit position
            Transform t = particle.transform;
            t.SetPositionAndRotation(hitPosition, faceRotation);
        }
    }

    private void PlayVFX(ParticleSystem particle)
    {
        // Activate the particle
        // Make sure the particle and all children are reset
        particle.gameObject.SetActive(true);
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
    }
}
