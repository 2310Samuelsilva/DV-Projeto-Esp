using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    public LevelData levelData;               // ScriptableObject with level settings
    public TransportData transportData;       // ScriptableObject with transport settings

    [Header("Scene References")]
    public Transform playerSpawnPoint;        // Where player starts

     
    [SerializeField] private GameObject worldManagerPrefab;
    
    private GameObject playerInstance;
    private WorldManager worldManager;

    private float distanceTravelled = 0f;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        playerSpawnPoint = GameObject.Find("PlayerSpawnPoint").transform;

        //worldManager
        LoadLevel();
    }

    public void LoadLevel()
    {
        if (levelData == null || transportData == null)
        {
            Debug.LogError("LevelManager: Missing LevelData or TransportData!");
            return;
        }

        // Apply global physics settings
        Physics2D.gravity = new Vector2(0, levelData.gravity);

        // Spawn player
        playerInstance = Instantiate(transportData.prefab, playerSpawnPoint.position, Quaternion.identity);
       
        // Spawn world manager
        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity);
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData);
        
    }

    void Update()
    {
        //TrackDistance();
        //HandleSpeedIncrease();
    }

    private void TrackDistance()
    {
        if (playerInstance != null)
        {
            distanceTravelled += (playerInstance.transform.position.x - lastPlayerPosition.x);
            lastPlayerPosition = playerInstance.transform.position;
        }
    }

    private void HandleSpeedIncrease()
    {
        if (worldManager != null)
        {
            worldManager.SetScrollSpeed(levelData.speedIncreaseRate * Time.deltaTime);
        }
    }

    public float GetDistanceTravelled()
    {
        return worldManager.DistanceTravelled();
    }
}