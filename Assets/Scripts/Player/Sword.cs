using UnityEngine;
using CameraShake;

public class Sword : MonoBehaviour
{
    [SerializeField] BounceShake.Params bounceShakeParams;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            CameraShaker.Shake(new BounceShake(bounceShakeParams));
        }
    }
}
