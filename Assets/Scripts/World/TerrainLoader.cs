using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainLoader : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private float chunkGap = 0f;
    [SerializeField] private float chunkYPosition = 0f;
    [SerializeField] private LevelData levelData;
    [SerializeField] private ChunkSettings chunkSettings;

    private Camera mainCamera;
    private List<ProceduralChunk> activeChunks = new List<ProceduralChunk>();
    private GameObject proceduralChunkPrefab;

    private float lastRightEdgeHeight = 0f;
    private int nextChunkIndex = 0; // Added chunk index

    public void Initialize(LevelData levelData)
    {
        if (levelData == null) return;
        this.levelData = levelData;
        mainCamera = Camera.main;
        this.chunkSettings = levelData.chunkSettings;
        this.proceduralChunkPrefab = levelData.proceduralChunkPrefab;
        nextChunkIndex = 0; // reset index on initialize
    }

    public void InitializeChunks()
    {
        ClearExistingChunks();

        for (int i = 0; i < levelData.activeChunksAmount; i++)
        {
            SpawnChunkAtNextPosition();
        }
    }


    private float GetCameraLeftEdge()
    {
        return mainCamera.transform.position.x - (mainCamera.orthographicSize * mainCamera.aspect);
    }

    private void ClearExistingChunks()
    {
        Debug.Log("ClearExistingChunks activeChunks.Count: " + activeChunks.Count);
        foreach (var chunk in activeChunks)
        {

            if (chunk != null)
            {
                if (Application.isPlaying)
                    Destroy(chunk.gameObject);
                else
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        if (chunk == null) return;
                        DestroyImmediate(chunk.gameObject);
                    };
            }
        }

        activeChunks.Clear();
        lastRightEdgeHeight = 0f;
        nextChunkIndex = 0; // reset index when clearing
    }

    private void SpawnChunkAtNextPosition()
    {
        if (chunkSettings == null || proceduralChunkPrefab == null)
        {
            Debug.LogWarning("Invalid chunk settings or prefab, skipping chunk spawn.");
            return;
        }

        Vector3 spawnPosition;

        // First chunk — align to camera’s left edge
        if (activeChunks.Count == 0)
        {
            float cameraLeftEdge = GetCameraLeftEdge();
            spawnPosition = new Vector3(cameraLeftEdge, chunkYPosition, 0f);
        }
        else
        {
            // Position right after the last chunk
            ProceduralChunk lastChunk = activeChunks[activeChunks.Count - 1];
            spawnPosition = new Vector3(
                lastChunk.transform.position.x + lastChunk.GetWidth() + chunkGap,
                chunkYPosition,
                0f
            );
        }

        GameObject chunk = Instantiate(proceduralChunkPrefab, spawnPosition, Quaternion.identity, transform);
        ProceduralChunk proceduralChunk = chunk.GetComponent<ProceduralChunk>();

        // Initialize chunk with chunkSettings and last height for smoothness
        proceduralChunk.Initialize(chunkSettings, lastRightEdgeHeight, levelData.seed, nextChunkIndex);

        activeChunks.Add(proceduralChunk);
        lastRightEdgeHeight = proceduralChunk.GetRightEdgeHeight();

        nextChunkIndex++; // increment index for the next chunk
    }

    public void MoveChunks(float scrollSpeed)
    {
        if (activeChunks.Count == 0) return;

        // Move all chunks
        foreach (var chunk in activeChunks)
        {
            chunk.transform.position += scrollSpeed * Time.deltaTime * Vector3.left;
        }

        // Check if chunk is offscreen
        HandleOffscreenChunks();
    }

    private void HandleOffscreenChunks()
    {
        ProceduralChunk leftmost = activeChunks[0];
        if (leftmost == null) return;

        float cameraLeftEdge = GetCameraLeftEdge();

        if (leftmost.transform.position.x + leftmost.GetWidth() < cameraLeftEdge)
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