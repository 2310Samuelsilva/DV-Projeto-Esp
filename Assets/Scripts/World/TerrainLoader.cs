using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainLoader : MonoBehaviour
{
    [SerializeField] private int activeChunksAmount = 5;
    [SerializeField] private float chunkGap = 0f;
    [SerializeField] private float chunkYPosition = 0f;

    private Camera mainCamera;
    [SerializeField] private LevelData levelData;

    private List<ProceduralChunk> activeChunks = new List<ProceduralChunk>();
    [SerializeField] private ChunkSettings chunkSettings;
    private GameObject proceduralChunkPrefab;

    private float nextChunkSpawnX = 0f;

    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;
        if (levelData == null) return;

        this.chunkSettings = levelData.chunkSettings;
        this.proceduralChunkPrefab = levelData.proceduralChunkPrefab;

        InitializeChunks();
    }

    public void InitializeChunks()
    {
        ClearExistingChunks();

        activeChunks.Clear();
        nextChunkSpawnX = 0f;

        for (int i = 0; i < activeChunksAmount; i++)
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
                if (Application.isPlaying)
                    Destroy(chunk.gameObject);
                else
                    DestroyImmediate(chunk.gameObject);
            }
        }
        activeChunks.Clear();
    }

    private void SpawnChunkAtNextPosition()
    {
        if (chunkSettings == null || proceduralChunkPrefab == null)
        {
            Debug.LogWarning("Invalid chunk settings or prefab, skipping chunk spawn.");
            return;
        }

        float width = chunkSettings.width;
        Vector3 spawnPos = new Vector3(nextChunkSpawnX + width / 2f, chunkYPosition, 0f);

        GameObject chunk = Instantiate(proceduralChunkPrefab, spawnPos, Quaternion.identity, transform);
        ProceduralChunk proceduralChunk = chunk.GetComponent<ProceduralChunk>();
        proceduralChunk.Initialize(chunkSettings);
        activeChunks.Add(proceduralChunk);

        nextChunkSpawnX += width + chunkGap;
    }

    public void MoveChunks(float scrollSpeed)
    {
        if (activeChunks.Count == 0) return;

        foreach (var chunk in activeChunks)
        {
            chunk.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        ProceduralChunk leftmost = activeChunks[0];
        if (leftmost == null) return;

        float cameraLeftEdge = mainCamera.transform.position.x - (mainCamera.orthographicSize * mainCamera.aspect);

        if (leftmost.transform.position.x + (leftmost.GetWidth() / 2f) < cameraLeftEdge)
        {
            activeChunks.RemoveAt(0);
            Destroy(leftmost.gameObject);
            SpawnChunkAtNextPosition();
        }
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
            mainCamera = Camera.main;
    }
}