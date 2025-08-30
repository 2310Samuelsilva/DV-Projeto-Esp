using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{

    [Header("Level Settings")]
    public string levelName;
    public string sceneName;
    public int levelNumber;
    public bool isUnlocked;
    public bool isCompleted;
    
    [Header("Level achievements")]
    public int scoreToUnlock;
    public int scoreToComplete;
    public int scoreToWin;
    public int bestScore;


    [Header("Level Environment")]
    public float baseScrollSpeed;
    public float scrollSpeedDecreaseRate;
    public float gravity = 9.81f;

    public ChunkSettings chunkSettings;
    public int seed;
    public int activeChunksAmount;
    public GameObject proceduralChunkPrefab;
    public GameObject avalanchePrefab;
    public float avalancheSpawnX = -20f;
    public float avalancheBaseSpeed = 1f;
    public float avalancheSpeedStep = 1.5f;
    public float avalancheDistanceStep = 100f;
    public float avalancheWobbleFrequency = 0.1f;
    public float avalancheWobbleAmplitude = 1f;
    public float fallThresholdY = -10f;

    public float speedIncreaseDistanceThreshold;
    public float speedIncreaseRate;
}

