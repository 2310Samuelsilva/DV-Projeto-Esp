using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Localization.Settings;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string savePath => Application.persistentDataPath + "/save.json";
    private string saveHistoryPath => Application.persistentDataPath + "/save_history.log";

    [Header("References")]
    public LevelList levelsList;
    public PlayerTransportDatabase transports;
    public PlayerData playerData;
    public GameOptions gameOptions;

    // Save

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Duplicate GameManager destroyed!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager Awake: instance set, will survive scene loads.");
    }

    private void WriteHistory(string message)
    {   

        if (!File.Exists(saveHistoryPath))
        {
            File.Create(saveHistoryPath);
        }

        DateTime utcTime = DateTime.UtcNow;
        string timeString = utcTime.ToString("yyyy-MM-dd HH:mm:ss");
        string content = $"[{timeString}] {message}";
        File.AppendAllText(saveHistoryPath, content + "\n");
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // Levels
        foreach (var level in levelsList.levels)
        {
            data.levelRecords.Add(new DictionaryStringInt { key = level.Id, value = level.bestScore });
            data.levelUnlocked.Add(new DictionaryStringBool { key = level.Id, value = level.isUnlocked });
        }

        // Transports
        foreach (var transport in transports.transports)
        {
            data.transportUnlocked.Add(new DictionaryStringBool { key = transport.Id, value = transport.IsUnlocked() });
            data.transportLevels.Add(new DictionaryStringInt { key = transport.Id, value = transport.GetLevel() });
            data.transportSelected.Add(new DictionaryStringBool { key = transport.Id, value = transport.IsSelected() });
        }


        data.gameOptionsVolume.Add(new DictionaryStringFloat { key = "Master", value = gameOptions.MasterVolume });
        data.gameOptionsVolume.Add(new DictionaryStringFloat { key = "Music", value = gameOptions.MusicVolumeRaw });
        data.gameOptionsVolume.Add(new DictionaryStringFloat { key = "SFX", value = gameOptions.SFXVolumeRaw });

        // Balance
        data.playerBalance = playerData.totalBalance;

        // Language
        data.selectedLanguage = gameOptions.Language;

        // Write file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to: " + savePath);

        // Write history
        WriteHistory("Game saved!");
    }

    // Load
    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found at: " + savePath);
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Levels
        foreach (var pair in data.levelRecords)
        {
            var level = levelsList.levels.Find(l => l.Id == pair.key);
            if (level != null) level.bestScore = pair.value;
        }
        foreach (var pair in data.levelUnlocked)
        {
            var level = levelsList.levels.Find(l => l.Id == pair.key);
            if (level != null) level.isUnlocked = pair.value;
        }

        // Transports
        foreach (var pair in data.transportUnlocked)
        {
            var transport = transports.transports.Find(t => t.Id == pair.key);
            if (transport != null) transport.SetUnlocked(pair.value);
        }
        foreach (var pair in data.transportLevels)
        {
            var transport = transports.transports.Find(t => t.Id == pair.key);
            if (transport != null) transport.SetLevel(pair.value);
        }
        foreach (var pair in data.transportSelected)
        {
            var transport = transports.transports.Find(t => t.Id == pair.key);
            if (transport != null) transport.SetSelected(pair.value);
        }

        // Game options
        gameOptions.MasterVolume = data.gameOptionsVolume.Find(v => v.key == "Master").value;
        gameOptions.MusicVolumeRaw = data.gameOptionsVolume.Find(v => v.key == "Music").value;
        gameOptions.SFXVolumeRaw = data.gameOptionsVolume.Find(v => v.key == "SFX").value;

        // Balance
        playerData.totalBalance = data.playerBalance;

        // Language
        if (!string.IsNullOrEmpty(data.selectedLanguage))
        {
            gameOptions.Language = data.selectedLanguage;

        } else
        {
            gameOptions.Language = "en";
        }

        Debug.Log("Game loaded from: " + savePath);

        // Write history
        WriteHistory("Game loaded!");
    }

     void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Game saved on exit!");
    }
}