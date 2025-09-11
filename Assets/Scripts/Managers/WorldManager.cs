using System.Collections;
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
    [SerializeField] private float scrollSpeed;                  // Current actual speed
    private float targetScrollSpeed;                             // Normal speed we recover toward
    [SerializeField] private float scrollSpeedRecoveryRate = 2f; // Units per second

    private float distanceTraveled;
    private float nextSpeedIncreaseDistance;

    [Header("Avalanche / Obstacle")]
    [SerializeField] private int maxHits = 3;                   // Max stacked hits
    [SerializeField] private float avalancheCloseDuration = 3f; // Seconds before avalanche resets after last hit

    private int currentHits = 0;
    private Coroutine avalancheCoroutine;
    private float remainingAvalancheTime = 0f;

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
        this.maxHits = levelData.avalancheMaxHits;
        this.avalancheCloseDuration = levelData.avalancheCloseDuration;

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
        currentHits = 0;
        remainingAvalancheTime = 0f;
        scrollSpeed = CalculateTargetScrollSpeed();
        terrainLoader.InitializeChunks();
    }

    // -------------------- Obstacle Interaction --------------------
    public void ObstacleHit()
    {

        if (currentHits >= maxHits)
        {
            LevelManager.Instance.EndLevel();
            return;
        }


        // Increase hits (max cap)
        currentHits = Mathf.Min(currentHits + 1, maxHits);


        UIGameplayManager.Instance.UpdateHistsUI((maxHits - currentHits).ToString());

        // Extend avalanche close duration
        remainingAvalancheTime += avalancheCloseDuration;

        // Make avalanche appear close
        avalancheController.SetClose(currentHits);
        DecreaseScrollSpeed();

        // Camera shake
        float amplitude = 1f * currentHits;
        float frequency = 1f * currentHits;
        EffectsManager.Instance.ShakeCamera("avalanche", amplitude, frequency, remainingAvalancheTime);

        // Start coroutine if not running
        if (avalancheCoroutine == null)
            avalancheCoroutine = StartCoroutine(AvalancheTimer());
    }

    private void DecreaseScrollSpeed()
    {
        float reduction = targetScrollSpeed * levelData.scrollSpeedDecreaseRate / 100f;
        scrollSpeed = Mathf.Max(scrollSpeed - reduction, levelData.baseScrollSpeed / 2f);
    }

    private IEnumerator AvalancheTimer()
    {
        

        while (remainingAvalancheTime > 0f)
        {
            remainingAvalancheTime -= Time.deltaTime;
            yield return null;
        }

        // Reset avalanche
        Debug.Log("Avalanche reset");
        avalancheController.Reset();
        currentHits = 0;
        UIGameplayManager.Instance.UpdateHistsUI(maxHits.ToString());
        avalancheCoroutine = null;
    }

    // -------------------- Update Loop --------------------
    private void Update()
    {
        if (!Application.isPlaying || LevelManager.Instance.IsPaused()) return;

        distanceTraveled += scrollSpeed * Time.deltaTime;

        terrainLoader.MoveChunks(scrollSpeed);
        avalancheController.UpdateAvalanche(scrollSpeed, distanceTraveled);

        IncreaseScrollSpeed();
        SmoothRecoverScrollSpeed();
        CheckForLoss();

        Debug.Log($"Distance Traveled: {distanceTraveled}, Scroll Speed: {scrollSpeed}");
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
            targetScrollSpeed += levelData.speedIncreaseRate * Time.deltaTime;
            nextSpeedIncreaseDistance += levelData.speedIncreaseDistanceThreshold;
        }
    }

    // -------------------- Loss Checks --------------------
    private void CheckForLoss()
    {
        if (playerController.transform.position.y < levelData.fallThresholdY)
            LevelManager.Instance.EndLevel();
    }

    // -------------------- Distance Info --------------------
    public float DistanceTravelled() => distanceTraveled;
}