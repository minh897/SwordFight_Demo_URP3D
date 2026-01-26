using Assets.Utility;
using UnityEngine;

public class PlayerPolish : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem vfxWeaponSwing;

    [Header("Sound Effects")]
    [SerializeField] private AudioStruct sfxWeaponSwing;

    private AudioSource audioSource;
    private PlayerAttackAnim attackAnim;

    private float swingVfxDir = 1;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        attackAnim = GetComponent<PlayerAttackAnim>();
    }

    void OnEnable()
    {
        attackAnim.OnAttackStarted += PlayAttackEffs;
        attackAnim.OnAttackFinished += FlipSwingVfx;
    }

    void OnDisable()
    {
        attackAnim.OnAttackStarted -= PlayAttackEffs;
        attackAnim.OnAttackFinished -= FlipSwingVfx;
    }

    public void PlayAttackEffs()
    {
        Utils.PlayVFX(vfxWeaponSwing);

        Utils.PlaySFX(
            audioSource, 
            sfxWeaponSwing.audio, 
            sfxWeaponSwing.volume, 
            sfxWeaponSwing.minPitch, 
            sfxWeaponSwing.maxPitch);
    }

    private void FlipSwingVfx()
    {
        // flip the vfx rotation in order to be in sync with the swing direction
        swingVfxDir = -swingVfxDir;
        vfxWeaponSwing.transform.localRotation = 
            swingVfxDir == 1 ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
    }
}
