using UnityEngine;

public class ChunkDataHolder : MonoBehaviour
{
    public ChunkData chunkData;

    public float GetChunkWidth()
    {
        return chunkData.width;
    }
}