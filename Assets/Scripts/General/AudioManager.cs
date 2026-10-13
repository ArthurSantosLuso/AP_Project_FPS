using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;
    [Range(0, 1)][SerializeField] private float musicVolume = 0.09f;
    [Range(0, 1)][SerializeField] private float sfxVolume = 1f;

    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
                FindFirstObjectByType<AudioManager>().Init();

            return _instance;
        }
    }

    private struct AudioSourceSetup
    {
        public bool musicExclusive;
        public AudioSource audioSource;

        public AudioSourceSetup(AudioSource source, bool isForMusic)
        {
            audioSource = source;
            musicExclusive = isForMusic;
        }
    }

    private List<AudioSourceSetup> audioSources;

    void Awake()
    {
        if (_instance == null)
            Init();
        else if (_instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        PlayMusic(sceneMusic);
    }

    private void Init()
    {
        _instance = this;
        //DontDestroyOnLoad(gameObject);  

        audioSources = new List<AudioSourceSetup>();
    }

    public void PlaySound(AudioClip audio, float pitch = 1f)
    {
        AudioSource audioSource;
        CheckForFreeAudioSource(out audioSource, false);
        if (audioSource == null)
            audioSource = CreateNewAudioSource(false);

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audio, sfxVolume);
    }

    public void PlayMusic(AudioClip audio)
    {
        AudioSource audioSource;
        CheckForFreeAudioSource(out audioSource, true);

        if (audioSource == null)
        {
            audioSource = CreateNewAudioSource(true);
        }
        audioSource.volume = musicVolume;
        audioSource.loop = true;
        audioSource.clip = audio;
        audioSource.Play();
    }

    public void CheckForFreeAudioSource(out AudioSource source, bool souldBeMusicExclusive)
    {
        foreach (AudioSourceSetup audioS in audioSources)
        {
            if (!audioS.audioSource.isPlaying && audioS.musicExclusive == souldBeMusicExclusive)
            {
                source = audioS.audioSource;
                return;
            }
        }

        source = null;
    }

    public AudioSource CreateNewAudioSource(bool isMusicExclusive)
    {
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        AudioSourceSetup audioSourceConfig = new AudioSourceSetup(newAudioSource, isMusicExclusive);
        audioSources.Add(audioSourceConfig);
        return audioSourceConfig.audioSource;
    }

    public AudioSource PlayLoopingSound(AudioClip audio)
    {
        AudioSource audioSource;
        CheckForFreeAudioSource(out audioSource, false);
        if (audioSource == null)
            audioSource = CreateNewAudioSource(false);

        audioSource.clip = audio;
        audioSource.loop = true;
        audioSource.volume = sfxVolume;
        audioSource.Play();
        return audioSource;
    }
}