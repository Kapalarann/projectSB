using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioOneShot;
    [SerializeField] private AudioSource audioLoop;

    [Header("SFX Clips")]
    [SerializeField] private SoundData walkSFX;
    [SerializeField] private SoundData slashSFX;
    [SerializeField] private SoundData blockSFX;
    [SerializeField] private SoundData parrySFX;
    [SerializeField] private SoundData dashSFX;
    [SerializeField] private SoundData hurtSFX;
    [SerializeField] private SoundData stunSFX;

    public void PlayWalkSound()
    {
        if (!audioLoop.isPlaying && walkSFX != null) // Prevent overlapping walk sounds
        {
            audioLoop.clip = walkSFX.clip;
            audioLoop.volume = walkSFX.volume;
            audioLoop.loop = true; // Walking should loop
            audioLoop.Play();
        }
    }

    public void StopWalkSound()
    {
        if (audioLoop.clip == walkSFX.clip) audioLoop.Stop();
    }

    public void PlaySlashSound()
    {
        if(slashSFX != null) audioOneShot.PlayOneShot(slashSFX.clip, slashSFX.volume);
    }

    public void PlayBlockSound()
    {
        if(blockSFX != null) audioOneShot.PlayOneShot(blockSFX.clip, blockSFX.volume);
    }

    public void PlayParrySound()
    {
        if (parrySFX != null) audioOneShot.PlayOneShot(parrySFX.clip, parrySFX.volume);
    }

    public void PlayDashSound()
    {
        if (dashSFX != null) audioOneShot.PlayOneShot(dashSFX.clip, dashSFX.volume);
    }

    public void PlayHurtSound()
    {
        if (hurtSFX != null) audioOneShot.PlayOneShot(hurtSFX.clip, hurtSFX.volume);
    }
    public void PlayStunSound()
    {
        if (hurtSFX != null) audioOneShot.PlayOneShot(stunSFX.clip, stunSFX.volume);
    }

    [System.Serializable]
    public class SoundData
    {
        public AudioClip clip;       // Reference to the audio clip
        [Range(0f, 1f)] public float volume = 1.0f;  // Volume (0 to 1)
    }
}
