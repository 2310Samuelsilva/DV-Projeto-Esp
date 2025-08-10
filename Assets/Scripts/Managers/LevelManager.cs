using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    public LevelData levelData;               // ScriptableObject with level settings
    public TransportData transportData;       // ScriptableObject with transport settings

    [Header("Scene References")]
    public Transform playerSpawnPoint;        // Where player starts

    private float nextSpeedIncreaseDistance;

     
    [SerializeField] private GameObject worldManagerPrefab;
    
    private GameObject playerInstance;
    private WorldManager worldManager;


    void Start()
    {
        playerSpawnPoint = GameObject.Find("PlayerSpawnPoint").transform;
        nextSpeedIncreaseDistance = levelData.speedIncreaseDistanceThreshold;
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
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();
        playerController.Initialize(transportData);
       
        // Spawn world manager
        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity);
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData);
        
    }

    void Update()
    {

        
        TrackDistance();
        HandleSpeedIncrease();
    }

    private void TrackDistance()
    {


        UIGameplayManager.Instance.UpdateDistanceUI(GetDistanceTravelled().ToString("F0"));
    }

   private void HandleSpeedIncrease()
{
    if (worldManager == null) return;

    if (GetDistanceTravelled() >= nextSpeedIncreaseDistance)
    {
        worldManager.IncreaseScrollSpeed();
        nextSpeedIncreaseDistance += levelData.speedIncreaseDistanceThreshold;
    }
}

    public float GetDistanceTravelled()
    {
        return worldManager.DistanceTravelled();
    }
}