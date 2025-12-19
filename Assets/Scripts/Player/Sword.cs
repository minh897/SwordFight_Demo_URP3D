using UnityEngine;
using CameraShake;

public class Sword : MonoBehaviour
{
    [SerializeField] private AudioClip sfxHitImpact;
    [SerializeField] BounceShake.Params bounceShakeParams;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            SoundFXManager.instance.PlaySoundFXClip(sfxHitImpact, transform, 1f);
            CameraShaker.Shake(new BounceShake(bounceShakeParams));
        }
    }
}
