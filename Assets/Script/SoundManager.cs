using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundCategory
{
    public string categoryName;
    public List<AudioClip> clips = new List<AudioClip>();
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private List<AudioClip> musicClips = new List<AudioClip>();

    [Header("SFX Categories")]
    [SerializeField] private List<SoundCategory> sfxCategories = new List<SoundCategory>();

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, List<AudioClip>> sfxDict;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitDictionaries()
    {
        musicDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, List<AudioClip>>();

        foreach (var clip in musicClips)
            musicDict[clip.name] = clip;

        foreach (var category in sfxCategories)
            sfxDict[category.categoryName] = category.clips;
    }

    // Play Music
    public void PlayMusic(string clipName, bool loop = true)
    {
        if (musicDict.TryGetValue(clipName, out var clip))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip '{clipName}' not found!");
        }
    }

    // Play random SFX from a category
    public void PlaySFX(string categoryName)
    {
        if (sfxDict.TryGetValue(categoryName, out var clips) && clips.Count > 0)
        {
            var randomClip = clips[Random.Range(0, clips.Count)];
            sfxSource.PlayOneShot(randomClip);
        }
        else
        {
            Debug.LogWarning($"SFX category '{categoryName}' not found or empty!");
        }
    }

    // Stop music
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Volume control
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
