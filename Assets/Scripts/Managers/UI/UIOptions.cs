using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class UIOptions : MonoBehaviour
{
    public static UIOptions Instance { get; private set; }

    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeText;

    [Header("Language")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    private GameOptions gameOptions;
    private List<Locale> availableLocales = new List<Locale>();

    /// <summary>
    /// Singleton pattern. Destroys any duplicate instances of this script that may exist,
    /// and assigns the instance variable to this object.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Called when the UIOptions component is initialized.
    /// It initializes the volume sliders and language dropdown by setting their values to the saved game options.
    /// It also attaches listeners to the sliders and dropdown to update the game options when the values change.
    /// </summary>
    private void Start()
    {
        gameOptions = GameManager.Instance.GetGameOptions();

        // --- Attach listeners ---
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        // --- Initialize volume ---
        masterVolumeSlider.value = gameOptions.MasterVolume;
        musicVolumeSlider.value = gameOptions.MusicVolume;
        sfxVolumeSlider.value = gameOptions.SFXVolume;

        masterVolumeText.text = (gameOptions.MasterVolume * 100.0f).ToString("F0") + "%";
        musicVolumeText.text = (gameOptions.MusicVolume * 100.0f).ToString("F0") + "%";
        sfxVolumeText.text = (gameOptions.SFXVolume * 100.0f).ToString("F0") + "%";

        // --- Initialize languages ---
        PopulateLanguages();
    }

    /// <summary>
    /// Populate the language dropdown with all available locales.
    /// The value of the dropdown is set to the current language saved in the game options.
    /// </summary>
    private void PopulateLanguages()
    {
        languageDropdown.ClearOptions();
        availableLocales = LocalizationSettings.AvailableLocales.Locales;

        List<string> displayNames = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < availableLocales.Count; i++)
        {
            var locale = availableLocales[i];
            displayNames.Add(locale.LocaleName);

            // match saved language code (like "en", "pt")
            if (locale.Identifier.Code == gameOptions.Language)
                currentIndex = i;
        }

        languageDropdown.AddOptions(displayNames);
        languageDropdown.value = currentIndex;
        languageDropdown.RefreshShownValue();
    }

    // -------------------- Volume --------------------
    private void OnMasterVolumeChanged(float value)
    {
        gameOptions.MasterVolume = value;
        masterVolumeText.text = (value * 100.0f).ToString("F0") + "%";
    }


    /// <summary>
    /// Called when the music volume slider value changes.
    /// Sets the MusicVolumeRaw property of the GameOptions to the given value,
    /// and updates the displayed music volume text.
    /// </summary>
    private void OnMusicVolumeChanged(float value)
    {
        gameOptions.MusicVolumeRaw = value;
        musicVolumeText.text = (value * 100.0f).ToString("F0") + "%";
    }
    /// <summary>
    /// Called when the SFX volume slider value changes.
    /// Sets the SFXVolumeRaw property of the GameOptions to the given value,
    /// and updates the displayed SFX volume text.
    /// </summary>
    /// <param name="value">The new value of the SFX volume slider (0-1)</param>

    private void OnSFXVolumeChanged(float value)
    {
        gameOptions.SFXVolumeRaw = value;
        sfxVolumeText.text = (value * 100.0f).ToString("F0") + "%";
    }

    // -------------------- Language --------------------


    /// <summary>
    /// Called when the language dropdown selection changes.
    /// </summary>
    /// <param name="index">The index of the selected language in the dropdown</param>
    private void OnLanguageChanged(int index)
    {
        // Assuming you already have a reference to LocalizationSettings.AvailableLocales.Locales
        Locale locale = LocalizationSettings.AvailableLocales.Locales[index];
        Debug.Log($"Language changed to {locale.LocaleName} ({locale.Identifier.Code})");
        gameOptions.Language = locale.Identifier.Code;
        StartCoroutine(SetLanguageCoroutine(locale));
    }

    /// <summary>
    /// Coroutine that changes the current language to the given locale.
    /// Waits for the Localization system to initialize, then sets SelectedLocale.
    /// </summary>
    private System.Collections.IEnumerator SetLanguageCoroutine(Locale locale)
    {
        // Ensure localization system is ready
        yield return UnityEngine.Localization.Settings.LocalizationSettings.InitializationOperation;

        if (locale != null)
        {
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale = locale;
            Debug.Log($"Language changed to {locale.LocaleName} ({locale.Identifier.Code})");
        }
        else
        {
            Debug.LogWarning("Tried to change to a null locale!");
        }
    }

    // -------------------- Misc --------------------
    public void ResetGame()
    {
        Debug.Log("UIOptions: Reset Game");
        GameManager.Instance.ResetProgress();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}