using System;
using UnityEditor.Search;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [Header("Game Data")]
    [SerializeField] private GameData gameData;
    private PlayerData playerData;
    private PlayerTransportDatabase playerTransportDatabase;
    LevelList levelList;
    [SerializeField] GameOptions gameOptions;



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

        playerData = gameData.GetPlayerData();
        Debug.Log($"Player name: {playerData.name}");
        playerTransportDatabase = gameData.GetPlayerTransportDatabase();
        levelList = gameData.GetLevelList();
    }

    private void Start()
    {


    }

    public void LoadLevel(string levelName)
    {
        LevelData levelData = levelList.GetLevelData(levelName);
        LoadLevel(levelData);
    }

    public void LoadLevel(LevelData levelData)
    {

        // Perform checks
        if (levelData == null)
        {
            Debug.LogError("LevelManager: Missing LevelData!");
            return;
        }
        if (levelList == null)
        {
            Debug.LogError("LevelManager: Missing LevelList!");
            return;
        }
        if (levelList.LevelExists(levelData) == false)
        {
            Debug.LogError("LevelManager: Level does not exist!");
            return;
        }

        if (levelData.isUnlocked == false)
        {
            Debug.LogError("LevelManager: Level is not unlocked!");
            return;
        }

        levelList.SetSelectedLevel(levelData);
        string sceneName = levelData.sceneName;
        Debug.Log($"Loading level {levelData.name}");

        SceneLoader.Instance.LoadSceneByName(sceneName);

        // Delay init until scene is ready
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.Initialize(levelData, playerData);
            }


            SceneLoader.Instance.OnSceneLoaded -= null;
        };
    }

    public void LoadSelectedLevel()
    {
        LoadLevel(levelList.GetSelectedLevel());
    }

    public void SelectLevel(string levelName)
    {
        LevelData levelData = levelList.GetLevelData(levelName);
        Debug.Log($"Selected level: {levelData.levelName}");
        levelList.SetSelectedLevel(levelData);
    }

    public void RestartLevel()
    {
        LoadLevel(levelList.GetSelectedLevel());
    }

    public void ReturnToMainMenu()
    {
        SceneLoader.Instance.LoadSceneByName("MainMenu");
    }

    public void LoadTransports()
    {
        SceneLoader.Instance.LoadSceneByName("UITransports");
    }

    public void EndGame(float distance)
    {

        // Unlock Levels
        levelList.CheckUnlockLevel(distance);
        Debug.Log("Game Over");

    }




    // --- Public API ---
    public PlayerData GetPlayerData() => playerData;
    public GameOptions GetGameOptions() => gameOptions;
    public LevelData GetSelectedLevel() => levelList.GetSelectedLevel();
    public LevelList GetLevelList() => levelList;

}