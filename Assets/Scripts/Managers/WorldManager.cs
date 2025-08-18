using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class WorldManager : MonoBehaviour
{
    [SerializeField] private GameObject terrainLoaderPrefab;
    [SerializeField] private LevelData levelData;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject avalanchePrefab;
    [SerializeField] private AvalancheController avalancheController;
    [SerializeField] private TerrainLoader terrainLoader;
    [SerializeField] private float scrollSpeed; // MoveSpeed
    private float distanceTraveled;
    private float nextSpeedIncreaseDistance;


    // Ability to preview in editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || levelData == null) return;

        // Delay so Unity is not in the middle of rendering
        EditorApplication.delayCall += () =>
        {
            if (this == null) return; // Object might be gone
            Initialize(levelData, playerController);
        };

    }
#endif



    public void Initialize(LevelData levelData, PlayerController playerController)
    {
        this.levelData = levelData;
        this.playerController = playerController;
        this.avalanchePrefab = levelData.avalanchePrefab;


        // For editopr map vizualization
        if (terrainLoader != null)
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(terrainLoader.gameObject);
            }
        }

        // Place self position on camera left edge
        float cameraLeftEdge = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);
        transform.position = new Vector3(cameraLeftEdge, 0, 0);


        GameObject tl = Instantiate(terrainLoaderPrefab, transform.position, Quaternion.identity, transform);
        terrainLoader = tl.GetComponent<TerrainLoader>();
        terrainLoader.Initialize(levelData);

        GameObject avalanche = Instantiate(avalanchePrefab, transform.position, Quaternion.identity, transform);
        avalancheController = avalanche.GetComponent<AvalancheController>();
        avalancheController.Initialize(levelData);

        Reset();

        Debug.Log("WorldManager initialized");
    }

    float CalculateScrollSpeed()
    {
        float scrollSpeed = levelData.baseScrollSpeed;
        scrollSpeed += playerController.MoveSpeed(); // Add player speed
        Debug.Log("ScrollSpeed: " + scrollSpeed);
        return scrollSpeed;
        
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
        scrollSpeed = CalculateScrollSpeed();
        terrainLoader.InitializeChunks();
    }

    private void Update()
    {
        
        if (Application.isPlaying)
        {

            CheckForLoss();

            Debug.Log("Moving chunks, scrollSpeed: " + scrollSpeed);
            distanceTraveled += scrollSpeed * Time.deltaTime;
            terrainLoader.MoveChunks(scrollSpeed);
            avalancheController.UpdateAvalanche(scrollSpeed);
            IncreaseScrollSpeed();
        }


    }

    private void CheckForLoss()
    {
        // Check if player fell off the map
        if (playerController.transform.position.y < levelData.fallThresholdY)
        {
            LevelManager.Instance.EndLevel();
        }

        // Check avalanche
        if (HasAvalancheHitPlayer())
        {
            LevelManager.Instance.EndLevel();
        }
    }

    private bool HasAvalancheHitPlayer()
    {
        float avalancheX = avalancheController.GetAvalanchePosition();
        if (avalancheX > playerController.transform.position.x)
        {
            return true;
        }

        return false;
    }

    public void IncreaseScrollSpeed()
    {
        if (DistanceTravelled() >= nextSpeedIncreaseDistance)
        {
            scrollSpeed += levelData.speedIncreaseRate * Time.deltaTime;
            nextSpeedIncreaseDistance += levelData.speedIncreaseDistanceThreshold;
        }

    }

    public float DistanceTravelled() => distanceTraveled;
    //public float ScrollSpeed() => scrollSpeed;
}