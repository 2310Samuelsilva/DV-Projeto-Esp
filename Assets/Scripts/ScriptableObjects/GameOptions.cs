using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "GameOptions", menuName = "Game/Options")]
public class GameOptions : ScriptableObject
{
    [SerializeField] private string id;
    public string Id => id;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("Language Settings")]
    [SerializeField] private string language = "en";
    [SerializeField] private List<string> availableLanguages = new List<string> { "en", "pt" };

    // --- Properties (safe access) ---
    public float MasterVolume { get => masterVolume; set => masterVolume = Mathf.Clamp01(value); }
    public float MusicVolumeRaw { get => musicVolume; set => musicVolume = Mathf.Clamp01(value); }
    public float SFXVolumeRaw { get => sfxVolume; set => sfxVolume = Mathf.Clamp01(value); }
    public string Language
    {
        get => language;
        set
        {
            language = value;
        }
    }
    public List<string> AvailableLanguages => availableLanguages;
    // -------------------- Initialization --------------------
    public void ApplySavedLanguage()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == language)
            {
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log($"[GameOptions] Applied saved language: {locale.LocaleName} ({locale.Identifier.Code})");
                return;
            }
        }

        // Fallback: use first locale if saved one not found
        if (locales.Count > 0)
        {
            LocalizationSettings.SelectedLocale = locales[0];
            language = locales[0].Identifier.Code;
            Debug.LogWarning($"[GameOptions] Saved language not found. Defaulting to {locales[0].LocaleName}");
        }
    }

    // --- Final applied volumes (affected by master) ---
    public float MusicVolume => masterVolume * musicVolume;
    public float SFXVolume => masterVolume * sfxVolume;
}