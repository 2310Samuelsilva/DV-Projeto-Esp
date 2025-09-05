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
    public ChunkSettings chunkSettings;
    public int seed;
    public int activeChunksAmount;
    public GameObject proceduralChunkPrefab;
    public GameObject avalanchePrefab;

    [Header("Movement Settings")]

    public float gravity = 9.81f;
    public float baseScrollSpeed;
    public float scrollSpeedDecreaseRate;
    public float speedIncreaseDistanceThreshold = 10f;
    public float speedIncreaseRate = 5f;
    public float fallThresholdY = -10f;

    [Header("Avalanche Settings")]
    public int avalancheMaxHits = 3;
    public float avalancheCloseDuration = 5f;
    public float avalancheSpawnX = -5f;
    public float avalancheWobbleFrequency = 0.1f;
    public float avalancheWobbleAmplitude = 1f;


    [Header("Sounds")]
    public AudioClip obstacleHitSFX;
}

