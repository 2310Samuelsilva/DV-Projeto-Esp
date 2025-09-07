using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    public static UIOptions Instance { get; private set; }

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeText;

    [SerializeField] private TMP_Dropdown languageDropdown;
    private GameOptions gameOptions;    


    private void Awake()
    {
        Debug.Log("UIOptions: Awake");
        if (Instance != null && Instance != this)
        {
            // Debug.Log("Duplicate UIShopManager destroyed!");
            // Destroy(gameObject);
            return;

        }
        Instance = this;
    }

    void Start()
    {
        this.gameOptions = GameManager.Instance.GetGameOptions();

        // Attach listeners
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        // Initialize UI
        masterVolumeSlider.value = gameOptions.MasterVolume;
        musicVolumeSlider.value = gameOptions.MusicVolume;
        sfxVolumeSlider.value = gameOptions.SFXVolume;

        masterVolumeText.text = (gameOptions.MasterVolume * 100.0).ToString("F0") + "%";
        musicVolumeText.text = (gameOptions.MusicVolume * 100.0).ToString("F0") + "%";
        sfxVolumeText.text = (gameOptions.SFXVolume * 100.0).ToString("F0") + "%";

        // Match dropdown to stored language
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(gameOptions.AvailableLanguages);
        languageDropdown.value = gameOptions.AvailableLanguages.IndexOf(gameOptions.Language);
    }


    private void OnMasterVolumeChanged(float value)
    {
        gameOptions.MasterVolume = value;
        masterVolumeText.text = (value * 100.0).ToString("F0") + "%";

        Debug.Log($"Master Volume set to {value}");
    }

    private void OnMusicVolumeChanged(float value)
    {
        gameOptions.MusicVolumeRaw = value;
        musicVolumeText.text = (value * 100.0).ToString("F0") + "%";
        Debug.Log($"Music Volume set to {value}");
    }

    private void OnSFXVolumeChanged(float value)
    {
        gameOptions.SFXVolumeRaw = value;
        sfxVolumeText.text = (value * 100.0).ToString("F0") + "%";
        Debug.Log($"SFX Volume set to {value}");
    }

    private void OnLanguageChanged(int index)
    {
        gameOptions.Language = gameOptions.AvailableLanguages[index];
        string selected = languageDropdown.options[index].text;
        Debug.Log($"Language set to {selected}");
    }


    public void ResetGame()
    {
        Debug.Log("UIOptions: Reset Game");
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}