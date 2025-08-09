
using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    public LevelData levelData;  // Assigned by LevelManager

    private GameObject[] groundChunks;
    private float chunkWidth;
    private int chunksOnScreen;
    private float scrollSpeed;

    private Transform[] activeChunks;

    void Start()
    {
        groundChunks = levelData.groundChunkPrefabs;
        chunkWidth = levelData.chunkWidth;
        chunksOnScreen = levelData.chunksOnScreen;
        scrollSpeed = levelData.scrollSpeed;

        activeChunks = new Transform[chunksOnScreen];
        InitializeGroundChunks();
    }

    void InitializeGroundChunks()
    {
        for (int i = 0; i < chunksOnScreen; i++)
        {
            GameObject chunk = Instantiate(groundChunks[0], new Vector3(i * chunkWidth, -2, 0), Quaternion.identity);
            activeChunks[i] = chunk.transform;
        }
    }

    void Update()
    {
        foreach (var chunk in activeChunks)
        {
            chunk.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        if (activeChunks[0].position.x < -chunkWidth)
        {
            Transform leftmost = activeChunks[0];
            for (int i = 0; i < activeChunks.Length - 1; i++)
                activeChunks[i] = activeChunks[i + 1];
            activeChunks[activeChunks.Length - 1] = leftmost;

            leftmost.position = activeChunks[activeChunks.Length - 2].position + Vector3.right * chunkWidth;

            int randomIndex = Random.Range(0, groundChunks.Length);
            Destroy(leftmost.gameObject);
            GameObject newChunk = Instantiate(groundChunks[randomIndex], leftmost.position, Quaternion.identity);
            activeChunks[activeChunks.Length - 1] = newChunk.transform;
        }
    }

    // You can add public methods to update scrollSpeed on the fly if needed
    public void SetScrollSpeed(float newSpeed)
    {
        scrollSpeed = newSpeed;
    }
}