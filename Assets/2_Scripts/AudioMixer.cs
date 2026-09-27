using UnityEngine;

public static class AudioMixer
{
    public static void PlayWithPitch(this AudioSource source, AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f, float volume = 1f)
    {
        if (source == null || clip == null) return;

        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clip, volume);
    }

}
