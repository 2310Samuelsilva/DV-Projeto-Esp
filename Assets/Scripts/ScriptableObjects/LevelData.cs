using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public float baseScrollSpeed;
    public float gravity = 9.81f;

    public ChunkSettings chunkSettings;
    public int activeChunksAmount;
    public GameObject proceduralChunkPrefab;

    public float speedIncreaseDistanceThreshold;
    public float speedIncreaseRate;
}

