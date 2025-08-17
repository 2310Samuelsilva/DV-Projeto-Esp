using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }

    [Header("Level Configuration")]
    [SerializeField] LevelData levelData;
    [SerializeField] PlayerData playerData;
    [SerializeField] PlayerTransportData transportData;

    [Header("Scene References")]
    [SerializeField] private Vector3 playerSpawnPoint;

    [SerializeField] private GameObject worldManagerPrefab;

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

    // Track the last data used for preview so we only refresh when needed
    private void Start()
    {
        playerSpawnPoint = Vector3.zero;
        if (Application.isPlaying)
        {
            LoadLevel();
        }
    }

    private void LoadLevel()
    {
        if (levelData == null || transportData == null)
        {
            Debug.LogError("LevelManager: Missing LevelData or TransportData!");
            return;
        }

        playerInstance = Instantiate(transportData.GetPrefab(), playerSpawnPoint, Quaternion.identity);
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();
        playerController.Initialize(transportData);

        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity);
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData, playerController);


    }


    // Game Update logic
    public void Update()
    {
        // Update UI
        string distance = worldManager.DistanceTravelled().ToString("F0") + "m";
        UIGameplayManager.Instance.UpdateDistanceUI(distance);
    }

    public void EndLevel()
    {
        Debug.Log("EndLevel");
        Respawn();
        worldManager.Reset();

    }

    private void Respawn()
    {
        playerInstance.transform.position = Vector3.zero;
        playerInstance.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Debug.Log("Player respawned.");
    }

    public LevelData GetLevelData()
    {
        return levelData;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }
}