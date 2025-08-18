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

    private void Start()
    {
        playerData = gameData.GetPlayerData();
        playerTransportDatabase = gameData.GetPlayerTransportDatabase();
        levelList = gameData.GetLevelList();
        
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

    public void EndGame()
    {
        Debug.Log("Game Over");
        SceneLoader.Instance.LoadSceneByName("GameWin");
    }


    // --- Public API ---
    public PlayerData GetPlayerData() => playerData;

}