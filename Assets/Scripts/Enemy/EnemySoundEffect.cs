using UnityEngine;

public class EnemySoundEffect : MonoBehaviour
{
    [SerializeField] private AudioClip sfxSwordHit;
    [SerializeField] private AudioClip sfxDeath;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField, Range(0, 1)] private float volume;

    private AudioSource sfxSource;

    void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
    }

    public void PlaySwordHitSFX()
    {
        PlaySFX(sfxSwordHit);
    }

    public void PlayDeathSFX()
    {
        PlaySFX(sfxDeath);
    }

    private void PlaySFX(AudioClip clip)
    {
        AudioManager.Instance.PlaySFX(clip, sfxSource, volume, minPitch, maxPitch);
    }
}
