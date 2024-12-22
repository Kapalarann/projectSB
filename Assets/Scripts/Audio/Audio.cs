using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource; // The main audio source

    [Header("SFX Clips")]
    [SerializeField] private SoundData walkSFX;   // Walking sound effect
    [SerializeField] private SoundData slashSFX; // Slashing sound effect
    [SerializeField] private SoundData dashSFX;  // Dashing sound effect
    [SerializeField] private SoundData hurtSFX;  // Hurt sound effect
    [SerializeField] private SoundData stunSFX;  // Hurt sound effect

    public void PlayWalkSound()
    {
        if (!audioSource.isPlaying && walkSFX != null) // Prevent overlapping walk sounds
        {
            audioSource.clip = walkSFX.clip;
            audioSource.volume = walkSFX.volume;
            audioSource.loop = true; // Walking should loop
            audioSource.Play();
        }
    }

    public void StopWalkSound()
    {
        if (audioSource.clip == walkSFX.clip) audioSource.Stop();
    }

    public void PlaySlashSound()
    {
        if(slashSFX != null) audioSource.PlayOneShot(slashSFX.clip, slashSFX.volume);
    }

    public void PlayDashSound()
    {
        if (dashSFX != null) audioSource.PlayOneShot(dashSFX.clip, dashSFX.volume);
    }

    public void PlayHurtSound()
    {
        if (hurtSFX != null) audioSource.PlayOneShot(hurtSFX.clip, hurtSFX.volume);
    }
    public void PlayStunSound()
    {
        if (hurtSFX != null) audioSource.PlayOneShot(stunSFX.clip, stunSFX.volume);
    }

    [System.Serializable]
    public class SoundData
    {
        public AudioClip clip;       // Reference to the audio clip
        [Range(0f, 1f)] public float volume = 1.0f;  // Volume (0 to 1)
    }
}
