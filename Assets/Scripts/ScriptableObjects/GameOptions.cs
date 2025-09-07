using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameOptions", menuName = "Game/Options")]
public class GameOptions : ScriptableObject
{
    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("Language Settings")]
    [SerializeField] private string language = "English";
    [SerializeField] private List<string> availableLanguages = new List<string> { "English", "Portugues" };

    // --- Properties (safe access) ---
    public float MasterVolume { get => masterVolume; set => masterVolume = Mathf.Clamp01(value); }
    public float MusicVolumeRaw { get => musicVolume; set => musicVolume = Mathf.Clamp01(value); }
    public float SFXVolumeRaw { get => sfxVolume; set => sfxVolume = Mathf.Clamp01(value); }
    public string Language
    {
        get => language;
        set
        {
            if (availableLanguages.Contains(value))
                language = value;
            else
                Debug.LogWarning($"Language '{value}' is not in the available languages list.");
        }
    }
    public List<string> AvailableLanguages => availableLanguages;

    // --- Final applied volumes (affected by master) ---
    public float MusicVolume => masterVolume * musicVolume;
    public float SFXVolume => masterVolume * sfxVolume;
}