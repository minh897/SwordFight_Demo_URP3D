using System.Collections;
using UnityEngine;

namespace Assets.Utility
{    
    public static class Utils
    {
        /// Smoothly interpolates a value from a starting point to a target point over a given duration. 
        /// The interpolated value is applied each frame via a callback (delegate), and the final target is
        /// guaranteed to be applied when the interpolation completes.
        public static IEnumerator LerpOverTime(
            float duration, 
            float start, 
            float end, 
            System.Action<float> applyLerpTo)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.fixedDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float newValue = Mathf.Lerp(start, end, t);
                applyLerpTo(newValue);
                yield return null;
            }
            // making sure the interpolated value reaches its target
            applyLerpTo(end); 
        }

        public static void PlayVFX(ParticleSystem particle)
        {
            // Make sure all particles are reset
            particle.gameObject.SetActive(true);
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play();
        }

        public static void PlaySFX(AudioSource sfxSource, AudioClip clip, float volume, float minPitch, float maxPitch)
        {
            if (!clip)
            {
                Debug.LogWarning("No audio clip to play");
                return;
            }

            sfxSource.pitch = Random.Range(minPitch, maxPitch);
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}
