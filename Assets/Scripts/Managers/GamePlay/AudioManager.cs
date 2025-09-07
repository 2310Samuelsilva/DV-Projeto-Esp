using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))] // Ensure an audio source is attached
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Background Music")]
    [SerializeField] private AudioClip introClip;   // Plays once
    [SerializeField] private AudioClip loopClip;    // Loops indefinitely
    [SerializeField] private float musicFadeDuration = 1f;

    [Header("Sound Effects")]
    [SerializeField] private List<AudioClip> sfxClips;  // Optional list for inspector

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private bool hasSwitchedToLoop = false;
    private bool calledStart = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        
        musicSource.loop = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        GameOptions gameOptions = GameManager.Instance.GetGameOptions();

        if (gameOptions == null)
        {
            Debug.LogError("GameOptions not found!");
            return;
        }


        musicSource.volume = gameOptions.MusicVolume;
        sfxSource.volume = gameOptions.SFXVolume;
    }

    public void StartMusic(){
        PlayMusicIntro();
        calledStart = true;
    }

    private void Update()
    {
        // Switch to loop music once intro finishes
        if (calledStart && !musicSource.isPlaying && !hasSwitchedToLoop)
        {
            PlayMusicLoop();
        }
    }

    // -------------------- Background Music --------------------
    public void PlayMusicIntro()
    {
        if (introClip != null)
        {
            musicSource.clip = introClip;
            musicSource.loop = false;
            musicSource.Play();
            hasSwitchedToLoop = false;
        }
    }

    public void PlayMusicLoop()
    {
        if (loopClip != null)
        {
            musicSource.clip = loopClip;
            musicSource.loop = true;
            musicSource.Play();
            hasSwitchedToLoop = true;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        hasSwitchedToLoop = false;
    }


    // -------------------- Sound Effects --------------------
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlaySFX(string clipName, float volume = 1f)
    {
        AudioClip clip = sfxClips.Find(c => c.name == clipName);
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }
}