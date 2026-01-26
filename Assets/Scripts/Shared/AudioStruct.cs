using UnityEngine;

[System.Serializable]
public struct AudioStruct
{
    public AudioClip audio;
    public float minPitch;
    public float maxPitch;
    [Range(0, 1)] public float volume;
}
