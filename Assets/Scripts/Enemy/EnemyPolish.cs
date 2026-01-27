using Assets.Utility;
using UnityEngine;

public class EnemyPolish : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem vfxHitSpark;
    [SerializeField] private ParticleSystem vfxEnemyDeath;

    [Header("Sound Effects")]
    [SerializeField] private AudioStruct sfxHitImpact;
    [SerializeField] private AudioStruct sfxEnemyExplosion;

    private ShakeCamera shakeCam;
    private AudioSource audioSource;
    private MakeTransparent makeTransparent;
    private EnemyDamageFlash damageFlash;
    private EnemyHitDetection hitDetection;

    void Awake()
    {
        shakeCam = FindFirstObjectByType<ShakeCamera>();
        audioSource = GetComponent<AudioSource>();
        makeTransparent = GetComponent<MakeTransparent>();
        hitDetection = GetComponent<EnemyHitDetection>();
        damageFlash = GetComponent<EnemyDamageFlash>();
    }

    void OnEnable()
    {
        hitDetection.OnImpact += PlayEnemyHitEffs;
        hitDetection.OnDead += PlayEnemyDeathEffs;
        hitDetection.OnCollectHitInfo += MoveHitSparkPosTo;
    }

    void OnDisable()
    {
        hitDetection.OnImpact -= PlayEnemyHitEffs;
        hitDetection.OnDead -= PlayEnemyDeathEffs;
        hitDetection.OnCollectHitInfo -= MoveHitSparkPosTo;
    }

    public void PlayEnemyHitEffs()
    {
        Utils.PlayVFX(vfxHitSpark);
        damageFlash.FlashColor();
        shakeCam.PlayBounceShake();

        Utils.PlaySFX(
            audioSource,
            sfxHitImpact.audio,
            sfxHitImpact.volume,
            sfxHitImpact.minPitch,
            sfxHitImpact.maxPitch
        );
    }

    public void PlayEnemyDeathEffs()
    {
        Utils.PlayVFX(vfxEnemyDeath);
        makeTransparent.SetMatToTransparent(damageFlash.Renderers);

        Utils.PlaySFX(
            audioSource,
            sfxEnemyExplosion.audio,
            sfxEnemyExplosion.volume,
            sfxEnemyExplosion.minPitch,
            sfxEnemyExplosion.maxPitch
        );
    }

    private void MoveHitSparkPosTo(Vector3 position, Collider collider)
    {
        // Move the hit spark vfx position to hit position
        Vector3 hitPosition = collider.ClosestPoint(position);
        Vector3 direction = (hitPosition - position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion faceRotation = Quaternion.LookRotation(direction);
            Transform t = vfxHitSpark.transform;
            t.SetPositionAndRotation(hitPosition, faceRotation);
        }
    }
}
