using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainLoader : MonoBehaviour
{
    [SerializeField] private float chunkGap = 0f;
    [SerializeField] private float chunkYPosition = 0f;

    private Camera mainCamera;
    [SerializeField] private LevelData levelData;

    private List<ProceduralChunk> activeChunks = new List<ProceduralChunk>();
    [SerializeField] private ChunkSettings chunkSettings;
    private GameObject proceduralChunkPrefab;

    private float nextChunkSpawnX = 0f;
    float lastRightEdgeHeight = 0f;
    
    public void Initialize(LevelData levelData)
    {
        if (levelData == null) return;
        this.levelData = levelData;
        mainCamera = Camera.main;
        this.chunkSettings = levelData.chunkSettings;
        this.proceduralChunkPrefab = levelData.proceduralChunkPrefab;

        //InitializeChunks();
    }

    public void InitializeChunks()
    {
        ClearExistingChunks();

        activeChunks.Clear();

        float cameraLeftEdge = mainCamera.transform.position.x - (mainCamera.orthographicSize * mainCamera.aspect);
        nextChunkSpawnX = cameraLeftEdge;
        for (int i = 0; i < levelData.activeChunksAmount; i++)
        {
            SpawnChunkAtNextPosition();
        }
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
                    Debug.Log("Desroying chunk: " + chunk);
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (chunk == null) return;
                    DestroyImmediate(chunk.gameObject);
                };
                    
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
        Vector3 spawnPos = new Vector3(nextChunkSpawnX , chunkYPosition, 0f);

        GameObject chunk = Instantiate(proceduralChunkPrefab, spawnPos, Quaternion.identity, transform);
        ProceduralChunk proceduralChunk = chunk.GetComponent<ProceduralChunk>();
        
        // Initialize chunk with chunkSettings and the last edge height
        proceduralChunk.Initialize(chunkSettings, lastRightEdgeHeight);

        activeChunks.Add(proceduralChunk);

        // Update lastRightEdgeHeight to the new chunk's right edge height
        lastRightEdgeHeight = proceduralChunk.GetRightEdgeHeight();

        nextChunkSpawnX += width + chunkGap;
    }

    public void MoveChunks(float scrollSpeed)
    {
        if (activeChunks.Count == 0) return;

        foreach (var chunk in activeChunks)
        {
            chunk.transform.position += scrollSpeed * Time.deltaTime * Vector3.left;
        }

        ProceduralChunk leftmost = activeChunks[0];
        if (leftmost == null) return;

        float cameraLeftEdge = mainCamera.transform.position.x - (mainCamera.orthographicSize * mainCamera.aspect);

        if (leftmost.transform.position.x + (leftmost.GetWidth()) < cameraLeftEdge)
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