using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public GameObject[] groundChunkPrefabs;
    public float chunkWidth = 10f;
    public int chunksOnScreen = 5;
    public float scrollSpeed = 5f;
    public float gravity = -9.81f;

    
    // Add more customizations here, e.g.:
    public float obstacleSpawnRate;
    public float avalancheCatchUpSpeed;
}