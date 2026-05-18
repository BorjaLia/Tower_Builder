using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("Mixer")]
    [Tooltip("The main AudioMixer of the project")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [System.Serializable]
    public struct SoundEffect
    {
        public string soundName;
        public AudioClip clip;
    }

    [Header("Audio Library")]
    [Tooltip("List of sound effects with their string names")]
    [SerializeField] private List<SoundEffect> sfxLibrary;

    [Tooltip("Default background music")]
    [SerializeField] private AudioClip defaultMusic;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
    }

    public void PlaySFX(string soundName)
    {
        AudioClip clipToPlay = null;

        foreach (var sfx in sfxLibrary)
        {
            if (sfx.soundName == soundName)
            {
                clipToPlay = sfx.clip;
                break;
            }
        }

        if (clipToPlay != null)
        {
            sfxSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX '{soundName}' was not found.");
        }
    }
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource.clip == musicClip) return;

        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlayUISound(AudioClip uiClip)
    {
        uiSource.PlayOneShot(uiClip);
    }
    public void SetVolume(string volumeName, float value)
    {
        float dB = (value > Mathf.Epsilon) ? Mathf.Log10(value) * 20f : -144.0f;
        mainMixer.SetFloat(volumeName, dB);
    }
}