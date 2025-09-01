using UnityEngine;

/// <summary>
/// Handles level loading, player spawn, world initialization, and level completion.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Data")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private PlayerData playerData;

    [Header("Scene References")]
    [SerializeField] private GameObject worldManagerPrefab;
    [SerializeField] private Vector3 playerSpawnPoint = Vector3.zero;

    private PlayerTransportData transportData;
    private GameObject playerInstance;
    private WorldManager worldManager;
    private CameraManager cameraManager;

    [SerializeField] protected GameObject levelEndUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cameraManager = Camera.main?.GetComponent<CameraManager>();
        if (cameraManager == null)
            Debug.LogWarning("LevelManager: CameraManager not found on Main Camera.");
    }

    /// <summary>Initialize the level with given data.</summary>
    public void Initialize(LevelData levelData, PlayerData playerData)
    {
        this.levelData = levelData;
        this.playerData = playerData;
        transportData = playerData?.selectedTransport;

        LoadLevel();
    }

    /// <summary>Load the level: spawn player, world, and hide LevelEnd UI.</summary>
    private void LoadLevel()
    {
        if (levelData == null || playerData == null || transportData == null)
        {
            Debug.LogError("LevelManager: Missing required data to load level.");
            return;
        }

        Debug.Log($"LevelManager: Loading {levelData.levelName} with {transportData.GetName()}");

        SpawnPlayer();
        SpawnWorld();
        HideLevelEndUI();
        UnPauseGame();
    }

    /// <summary>Spawn the player prefab and initialize controller.</summary>
    private void SpawnPlayer()
    {
        if (playerInstance != null) Destroy(playerInstance);

        playerInstance = Instantiate(transportData.GetPrefab(), playerSpawnPoint, Quaternion.identity);
        var playerController = playerInstance.GetComponent<PlayerController>();
        playerController.Initialize(transportData);

        // Set Cinemachine target
        //cameraManager?.SetTarget(playerController.transform);
    }

    /// <summary>Instantiate world manager and initialize it.</summary>
    private void SpawnWorld()
    {
        if (worldManager != null) Destroy(worldManager.gameObject);

        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity);
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData, playerInstance.GetComponent<PlayerController>());
    }

    private void Update()
    {
        if (worldManager == null) return;

        // Update distance UI
        float distance = worldManager.DistanceTravelled();
        UIGameplayManager.Instance.UpdateDistanceUI($"{distance:F0}m");

        // Check Escape key to return to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.ReturnToMainMenu();
    }

    /// <summary>Called when player hits an obstacle.</summary>
    public void ObstacleHit()
    {
        worldManager?.DecreaseScrollSpeed();
    }

    /// <summary>End the level, calculate rewards, and show LevelEnd UI.</summary>
    public void EndLevel()
    {
        if (worldManager == null) return;

        PauseGame();
        float distance = worldManager.DistanceTravelled();
        int coinsEarned = CalculateCoinsEarned(distance);
        playerData.AddTotalBalance(coinsEarned);

        ShowLevelEndUI();
        UILevelEndManager.Instance.PopulateUI($"{distance:F0}m", coinsEarned);

        Debug.Log($"Level ended. Distance: {distance:F0}, Coins Earned: {coinsEarned}");
    }

    /// <summary>Calculate coins earned based on distance traveled.</summary>
    private int CalculateCoinsEarned(float distance) => Mathf.FloorToInt(distance / 100f);

    /// <summary>Respawn player at spawn point.</summary>
    private void Respawn()
    {
        if (playerInstance == null) return;

        playerInstance.transform.position = playerSpawnPoint;
        var rb = playerInstance.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Debug.Log("Player respawned.");
    }



    // -------------------- UI --------------------

    public void PauseGame() => Time.timeScale = 0;
    public void UnPauseGame() => Time.timeScale = 1;
    public void ShowLevelEndUI() => levelEndUI.SetActive(true);
    public void HideLevelEndUI() => levelEndUI.SetActive(false);

    // -------------------- Public Accessors --------------------
    public LevelData GetLevelData() => levelData;
    public PlayerTransportData GetTransportData() => transportData;
}