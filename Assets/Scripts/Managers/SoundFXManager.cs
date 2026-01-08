using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlaySFX(AudioClip audioClip, Transform spawnTransform, float volume, float minPitch, float maxPitch)
    {
        // spawn in gameObject with audio source
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        // assign the audioClip
        audioSource.clip = audioClip;
        // assign volume
        audioSource.volume = volume;
        // randomize pitch
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        // play sound
        audioSource.Play();
        // get length of sound FX clip
        float clipLength = audioSource.clip.length;
        // destroy the clip after it's done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
