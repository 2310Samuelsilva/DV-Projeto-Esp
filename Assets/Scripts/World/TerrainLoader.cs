using System.Collections.Generic;
using UnityEngine;

public class TerrainLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] chunkPrefabs;  // Assign in inspector
    [SerializeField] private int chunksOnScreen = 5;
    [SerializeField] private float chunkGap = 0.5f;
    [SerializeField] private float chunkYPosition = -2f;

    private LevelData levelData;

    public Transform[] activeChunks;
    public Dictionary<ChunkData, float> chunkWidths = new Dictionary<ChunkData, float>();

    public void Initialize(LevelData levelData)
    {
        Debug.Log("TerrainLoader: Initialize");
        this.levelData = levelData;
        this.chunkPrefabs = levelData.chunkPrefabs;

        CacheChunkWidths();
        //InitializeChunks();
    }

    public void InitializeChunks()
    {
        Reset();
        activeChunks = new Transform[chunksOnScreen];
        float spawnX = 0f;

        for (int i = 0; i < chunksOnScreen; i++)
        {
            GameObject prefab = chunkPrefabs[0]; // Start with first prefab or randomize if you want
            float width = chunkWidths[GetChunkData(prefab)];

            GameObject chunk = Instantiate(prefab, new Vector3(spawnX, chunkYPosition, 0f), Quaternion.identity, transform);
            activeChunks[i] = chunk.transform;

            spawnX += width + chunkGap;
        }
    }

    private void Reset()
    {
        this.activeChunks = null;
    }


    private void CacheChunkWidths()
    {
        Debug.Log("TerrainLoader: CacheChunkWidths");
        chunkWidths.Clear();

        foreach (var prefab in chunkPrefabs)
        {   
            Debug.Log("TerrainLoader: prefab: " + prefab);
            float width = GetChunkWidth(prefab);
            ChunkData chunkData = GetChunkData(prefab);
            chunkWidths[chunkData] = width;
        }
        Debug.Log("TerrainLoader: chunkWidths: " + chunkWidths);
    }

    private ChunkData GetChunkData(GameObject chunk)
    {
        return chunk.GetComponent<ChunkDataHolder>().chunkData;
    }

    private float GetChunkWidth(GameObject prefab)
    {
        ChunkDataHolder chunkDataHolder = prefab.GetComponent<ChunkDataHolder>();
        if (chunkDataHolder != null)
            return chunkDataHolder.GetChunkWidth();

        Debug.LogWarning("ChunkDataHolder missing on prefab: " + prefab.name);
        return 10f; 
    }

    public void MoveChunks(float scrollSpeed)
    {
        // Move all chunks left
        foreach (var chunk in activeChunks)
        {
            chunk.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        // Check if leftmost chunk went off-screen (fully past the left side)
        Transform leftmost = activeChunks[0];
        float leftmostWidth = chunkWidths[GetChunkData(leftmost.gameObject)];
        if (leftmost.position.x + leftmostWidth < 0f)
        {
            Debug.Log("Leftmost chunk went off-screen, witdth: " + leftmost.position.x);
            Debug.Log("Leftmost chunk went off-screen: " + leftmostWidth);
            ShiftChunks();
            LoadNewChunk();

            // Destroy the old leftmost chunk
            Destroy(leftmost.gameObject);
        }
    }

    private void ShiftChunks()
    {
        // Shift all chunks left in the array
        for (int i = 0; i < activeChunks.Length - 1; i++)
        {
            activeChunks[i] = activeChunks[i + 1];
        }
    }
    
    /*
    * Instantiates a new chunk at the position right of the last active chunk,
    * using a random prefab from the available chunkPrefabs. Updates the last
    * element of the activeChunks array to reference the newly instantiated chunk.
    */
    private void LoadNewChunk()
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Length);
        GameObject newChunk = Instantiate(chunkPrefabs[randomIndex], activeChunks[activeChunks.Length - 1].position + Vector3.right * chunkWidths[GetChunkData(chunkPrefabs[randomIndex])], Quaternion.identity);
        activeChunks[activeChunks.Length - 1] = newChunk.transform;
    }
}