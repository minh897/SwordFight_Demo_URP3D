using UnityEngine;
using CameraShake;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField] BounceShake.Params bounceShakeParams;

    public void PlayShortShake3D()
    {
        CameraShaker.Presets.ShortShake3D();
    }

    public void PlayBounceShake()
    {
        CameraShaker.Shake(new BounceShake(bounceShakeParams));
    }
}
