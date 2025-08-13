using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    public LevelData levelData;
    public TransportData transportData;

    [Header("Scene References")]
    public Transform playerSpawnPoint;

    [SerializeField] private GameObject worldManagerPrefab;

    private GameObject playerInstance;
    private WorldManager worldManager;

    [SerializeField] private Avalanche avalanche;


    // Track the last data used for preview so we only refresh when needed
    private void Start()
    {
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

        playerInstance = Instantiate(transportData.prefab, playerSpawnPoint.position, Quaternion.identity);
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();
        playerController.Initialize(transportData);

        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity);
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData);
    }

    public void Update()
    {
        string distance = worldManager.DistanceTravelled().ToString("F0") + "m";
        UIGameplayManager.Instance.UpdateDistanceUI(distance);
        avalanche.UpdateAvalanche(worldManager.ScrollSpeed());
    }
}