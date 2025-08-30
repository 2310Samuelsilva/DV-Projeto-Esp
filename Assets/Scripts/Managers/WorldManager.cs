using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class WorldManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject terrainLoaderPrefab;
    [SerializeField] private LevelData levelData;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject avalanchePrefab;

    private AvalancheController avalancheController;
    private TerrainLoader terrainLoader;

    [Header("Scrolling")]
    [SerializeField] private float scrollSpeed;          // Current actual speed
    private float targetScrollSpeed;                     // Normal speed we recover toward
    [SerializeField] private float scrollSpeedRecoveryRate = 2f; // Units per second

    private float distanceTraveled;
    private float nextSpeedIncreaseDistance;

    // -------------------- Editor Preview --------------------
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || levelData == null) return;

        EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            Initialize(levelData, playerController);
        };
    }
#endif

    // -------------------- Initialization --------------------
    public void Initialize(LevelData levelData, PlayerController playerController)
    {
        this.levelData = levelData;
        this.playerController = playerController;
        this.avalanchePrefab = levelData.avalanchePrefab;

        // Remove previous terrain loader in editor
        if (terrainLoader != null && !Application.isPlaying)
            DestroyImmediate(terrainLoader.gameObject);

        // Place WorldManager at camera left edge
        float cameraLeftEdge = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);
        transform.position = new Vector3(cameraLeftEdge, 0f, 0f);

        // Instantiate terrain loader
        GameObject tl = Instantiate(terrainLoaderPrefab, transform.position, Quaternion.identity, transform);
        terrainLoader = tl.GetComponent<TerrainLoader>();
        terrainLoader.Initialize(levelData);

        // Instantiate avalanche
        GameObject avalanche = Instantiate(avalanchePrefab, transform.position, Quaternion.identity, transform);
        avalancheController = avalanche.GetComponent<AvalancheController>();
        avalancheController.Initialize(levelData);

        Reset();
        Debug.Log("WorldManager initialized");
    }

    // -------------------- Scroll Speed Calculation --------------------
    private float CalculateTargetScrollSpeed()
    {
        targetScrollSpeed = levelData.baseScrollSpeed + playerController.MoveSpeed();
        return targetScrollSpeed;
    }

    public void Reset()
    {
        distanceTraveled = 0f;
        nextSpeedIncreaseDistance = levelData.speedIncreaseDistanceThreshold;

        if (playerController == null || levelData == null)
        {
            Debug.LogError("WorldManager: Missing PlayerController or LevelData!");
            return;
        }

        avalancheController.Reset();
        scrollSpeed = CalculateTargetScrollSpeed();
        terrainLoader.InitializeChunks();
    }

    // -------------------- Obstacle Interaction --------------------
    public void DecreaseScrollSpeed()
    {
        float reduction = targetScrollSpeed * levelData.scrollSpeedDecreaseRate / 100f;
        scrollSpeed = Mathf.Max(scrollSpeed - reduction, levelData.baseScrollSpeed/2f);// instant drop
    }

    // -------------------- Update Loop --------------------
    private void FixedUpdate()
    {
        if (!Application.isPlaying) return;

        distanceTraveled += scrollSpeed * Time.deltaTime;

        terrainLoader.MoveChunks(scrollSpeed);
        avalancheController.UpdateAvalanche(distanceTraveled);

        IncreaseScrollSpeed();
        SmoothRecoverScrollSpeed();
        CheckForLoss();
    }

    private void SmoothRecoverScrollSpeed()
    {
        if (scrollSpeed < targetScrollSpeed)
        {
            scrollSpeed += scrollSpeedRecoveryRate * Time.deltaTime;
            if (scrollSpeed > targetScrollSpeed)
                scrollSpeed = targetScrollSpeed;
        }
    }

    private void IncreaseScrollSpeed()
    {
        if (distanceTraveled >= nextSpeedIncreaseDistance)
        {
            scrollSpeed += levelData.speedIncreaseRate * Time.deltaTime;
            nextSpeedIncreaseDistance += levelData.speedIncreaseDistanceThreshold;
        }
    }

    // -------------------- Loss Checks --------------------
    private void CheckForLoss()
    {
        if (playerController.transform.position.y < levelData.fallThresholdY)
            LevelManager.Instance.EndLevel();

        if (HasAvalancheHitPlayer())
            LevelManager.Instance.EndLevel();
    }

    private bool HasAvalancheHitPlayer()
    {
        float avalancheX = avalancheController.GetAvalanchePosition();
        return avalancheX > playerController.transform.position.x;
    }

    // -------------------- Distance Info --------------------
    public float DistanceTravelled() => distanceTraveled;
}