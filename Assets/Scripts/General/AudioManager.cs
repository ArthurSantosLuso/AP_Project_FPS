using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;
    [Range(0, 1)][SerializeField] private float musicVolume = 0.09f;
    [Range(0, 1)][SerializeField] private float sfxVolume = 1f;

    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private readonly List<AudioSource> sfxSources = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
    }

    private void Start()
    {
        if (Instance == this) PlayMusic(sceneMusic);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetFreeSfxSource();
        source.loop = false;
        source.pitch = pitch;
        source.volume = 1f;                    // sfxVolume is applied once, by PlayOneShot
        source.PlayOneShot(clip, sfxVolume);
    }

    public AudioSource PlayLoopingSound(AudioClip clip)
    {
        AudioSource source = GetFreeSfxSource();
        source.clip = clip;
        source.loop = true;
        source.pitch = 1f;
        source.volume = sfxVolume;
        source.Play();
        return source;
    }

    private AudioSource GetFreeSfxSource()
    {
        foreach (AudioSource s in sfxSources)
            if (!s.isPlaying) return s;

        AudioSource created = gameObject.AddComponent<AudioSource>();
        sfxSources.Add(created);
        return created;
    }
}