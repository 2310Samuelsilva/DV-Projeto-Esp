using System.Collections.Generic;
using UnityEngine;

public class TerrainLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] chunkPrefabs;  // Assign in inspector or via LevelData
    [SerializeField] private int chunksOnScreen = 5;
    [SerializeField] private float chunkGap = 0f;
    [SerializeField] private float chunkYPosition = -2f;

    private Camera mainCamera;
    private LevelData levelData;

    private List<Transform> activeChunks = new List<Transform>();
    private Dictionary<ChunkData, float> chunkWidths = new Dictionary<ChunkData, float>();

    // Track where the next chunk should be spawned on the X axis
    private float nextChunkSpawnX = 0f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;
        this.chunkPrefabs = levelData.chunkPrefabs;

        CacheChunkWidths();
        //InitializeChunks();
    }

    public void InitializeChunks()
    {
        ClearExistingChunks();

        activeChunks.Clear();
        nextChunkSpawnX = 0f;

        for (int i = 0; i < chunksOnScreen; i++)
        {
            SpawnChunkAtNextPosition();
        }
    }

    private void ClearExistingChunks()
    {
        foreach (var chunk in activeChunks)
        {
            if (chunk != null)
            {
                Destroy(chunk.gameObject);
            }
        }
        activeChunks.Clear();
    }

    private void CacheChunkWidths()
    {
        chunkWidths.Clear();

        foreach (var prefab in chunkPrefabs)
        {
            var holder = prefab.GetComponent<ChunkDataHolder>();
            if (holder == null)
            {
                Debug.LogWarning($"ChunkDataHolder missing on prefab: {prefab.name}");
                continue;
            }
            ChunkData chunkData = GetChunkData(prefab);
            if (chunkData == null)
            {
                Debug.LogWarning($"ChunkData null on prefab: {prefab.name}");
                continue;
            }

            float width = chunkData.width;;
            if (!chunkWidths.ContainsKey(chunkData))
                chunkWidths.Add(chunkData, width);
        }
    }

    private ChunkData GetChunkData(GameObject obj)
    {
        var holder = obj.GetComponent<ChunkDataHolder>();
        if (holder == null)
        {
            Debug.LogWarning("ChunkDataHolder missing on prefab " + obj.name);
            return null;
        }
        return holder.ChunkData;
    }

    private void SpawnChunkAtNextPosition()
    {
        if (chunkPrefabs == null || chunkPrefabs.Length == 0)
        {
            Debug.LogWarning("No chunk prefabs assigned!");
            return;
        }

        int randomIndex = Random.Range(0, chunkPrefabs.Length);
        GameObject prefab = chunkPrefabs[randomIndex];




        ChunkData chunkData = GetChunkData(prefab);
        if (chunkData == null || !chunkWidths.ContainsKey(chunkData))
        {
            Debug.LogWarning("Invalid chunk data or width, skipping chunk spawn.");
            return;
        }

        float width = chunkWidths[chunkData];
        Vector3 spawnPos = new Vector3(nextChunkSpawnX + width / 2f, chunkYPosition, 0f);

        GameObject newChunk = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        activeChunks.Add(newChunk.transform);

        nextChunkSpawnX += width + chunkGap;
    }

    public void MoveChunks(float scrollSpeed)
    {
        if (activeChunks.Count == 0) return;

        foreach (var chunk in activeChunks)
        {
            chunk.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        // Check if leftmost chunk went off-screen
        Transform leftmost = activeChunks[0];
        ChunkData leftmostData = GetChunkData(leftmost.gameObject);
        if (leftmostData == null || !chunkWidths.ContainsKey(leftmostData)) return;

        float leftmostWidth = chunkWidths[leftmostData];
        float cameraLeftEdge = mainCamera.transform.position.x - (mainCamera.orthographicSize * mainCamera.aspect);

        // The chunk is fully offscreen if its right edge is left of camera left edge
        if (leftmost.position.x + (leftmostWidth / 2f) < cameraLeftEdge)
        {
            // Remove leftmost chunk and destroy it
            activeChunks.RemoveAt(0);
            Destroy(leftmost.gameObject);

            // Spawn new chunk at right
            SpawnChunkAtNextPosition();
        }
    }
}