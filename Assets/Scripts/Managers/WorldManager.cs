using UnityEngine;

[ExecuteAlways]
public class WorldManager : MonoBehaviour
{
    [SerializeField] private GameObject terrainLoaderPrefab;
    private LevelData levelData;

    [SerializeField] private float scrollSpeed;
    private float distanceTraveled;

    private TerrainLoader terrainLoader;

    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;

        if (terrainLoader != null)
        {
            if (Application.isPlaying)
                Destroy(terrainLoader.gameObject);
            else
                DestroyImmediate(terrainLoader.gameObject);
        }

        GameObject tl = Instantiate(terrainLoaderPrefab, Vector3.zero, Quaternion.identity, transform);
        terrainLoader = tl.GetComponent<TerrainLoader>();
        terrainLoader.Initialize(levelData);
        Reset();
    }

    private void Reset()
    {
        distanceTraveled = 0f;
        scrollSpeed = levelData != null ? levelData.baseScrollSpeed : 0f;
        if (terrainLoader != null)
            terrainLoader.InitializeChunks();
    }

    private void Update()
    {
        if (Application.isPlaying && terrainLoader != null)
        {
            distanceTraveled += scrollSpeed * Time.deltaTime;
            terrainLoader.MoveChunks(scrollSpeed);
        }
    }

    public void IncreaseScrollSpeed()
    {
        scrollSpeed += levelData.speedIncreaseRate * Time.deltaTime;
    }

    public float DistanceTravelled() => distanceTraveled;
}