using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Data")]
    [SerializeField] private LevelData levelData;   // The currently loaded level
    [SerializeField] private PlayerData playerData; // Persistent player data
    [SerializeField] private PlayerTransportData transportData; // Selected transport

    [Header("Scene References")]
    [SerializeField] private GameObject worldManagerPrefab;
    [SerializeField] private Vector3 playerSpawnPoint = Vector3.zero;

    private GameObject playerInstance;
    private WorldManager worldManager;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;



    }

    public void Initialize(LevelData levelData, PlayerData playerData)
    {
        Debug.Log("LevelManager: Initialize");
        this.levelData = levelData;
        this.playerData = playerData;
        this.transportData = playerData.selectedTransport;

        LoadLevel();
    }

    private void LoadLevel()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelManager: Missing LevelData!");
            return;
        }
        if (playerData == null)
        {
            Debug.LogError("LevelManager: Missing PlayerData!");
            return;
        }
        if (transportData == null)
        {
            transportData = playerData.selectedTransport; // fallback: use selected
        }

        Debug.Log($"LevelManager: Loading {levelData.levelName} with {transportData.GetName()}");

        // --- Spawn Player ---
        playerInstance = Instantiate(transportData.GetPrefab(), playerSpawnPoint, Quaternion.identity);
        var playerController = playerInstance.GetComponent<PlayerController>();
        playerController.Initialize(transportData);

        // set cinemachine tracking
        //FindAnyObjectByType<CameraManager>().SetTarget(playerController.transform);

        // --- Spawn World ---
        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity);
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData, playerController);


        Debug.Log($"LevelManager: Loaded {levelData.levelName} with {transportData.GetName()}");
    }

    private void Update()
    {
        if (worldManager == null) return;

        // Update UI
        float distance = worldManager.DistanceTravelled();
        UIGameplayManager.Instance.UpdateDistanceUI($"{distance:F0}m");
    }


    public void ObstacleHit()
    {
        this.worldManager.DecreaseScrollSpeed();
    }

    public void EndLevel()
    {
        Debug.Log("LevelManager: EndLevel triggered.");
        Respawn();
        worldManager?.Reset();
    }

    private void Respawn()
    {
        if (playerInstance == null) return;

        playerInstance.transform.position = playerSpawnPoint;
        var rb = playerInstance.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Debug.Log("Player respawned.");
    }

    // --- Public API ---
    public LevelData GetLevelData() => levelData;
    public PlayerTransportData GetTransportData() => transportData;
    
}