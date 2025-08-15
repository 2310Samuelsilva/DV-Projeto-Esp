using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public float baseScrollSpeed;
    public float gravity = 9.81f;

    public ChunkSettings chunkSettings;
    public int seed;
    public int activeChunksAmount;
    public GameObject proceduralChunkPrefab;
    public GameObject avalanchePrefab;
    public float avalancheSpawnX = -20f;
    public float avalancheSpeed = 10f;
    public float fallThresholdY = -10f;

    public float speedIncreaseDistanceThreshold;
    public float speedIncreaseRate;
}

