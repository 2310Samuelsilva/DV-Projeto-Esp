using Unity.Burst.CompilerServices;
using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    public GameObject[] groundChunks;
    public float chunkWidth = 10f;
    public int chunksOnScreen = 5;
    public float scrollSpeed = 5f;    // World speed


    private Transform[] activeChunks;

    void Start()
    {
        activeChunks = new Transform[chunksOnScreen];
        // Initialize ground chunks
        InitializeGroundChunks();
        
    }

    void InitializeGroundChunks()
    {
        for (int i = 0; i < chunksOnScreen; i++)
        {
            GameObject chunk = Instantiate(groundChunks[0], new Vector3(i * chunkWidth, 0, 0), Quaternion.identity);
            activeChunks[i] = chunk.transform;
        }
    }

   void Update()
    {
        // Move all chunks left
        foreach (var chunk in activeChunks)
        {
            chunk.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        // Check if leftmost chunk went off-screen
        if (activeChunks[0].position.x < -chunkWidth)
        {
            // Move it to the rightmost position
            Transform leftmost = activeChunks[0];
            for (int i = 0; i < activeChunks.Length - 1; i++)
            {
                activeChunks[i] = activeChunks[i + 1];
            }
            activeChunks[activeChunks.Length - 1] = leftmost;

            leftmost.position = activeChunks[activeChunks.Length - 2].position + Vector3.right * chunkWidth;

            // Optionally change chunk type for variety
            int randomIndex = Random.Range(0, groundChunks.Length);
            Destroy(leftmost.gameObject);
            GameObject newChunk = Instantiate(groundChunks[randomIndex], leftmost.position, Quaternion.identity);
            activeChunks[activeChunks.Length - 1] = newChunk.transform;
        }
    }
}