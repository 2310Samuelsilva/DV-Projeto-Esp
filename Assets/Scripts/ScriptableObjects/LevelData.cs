using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{

    [Header("Level Settings")]
    [SerializeField] public string levelName;
    [SerializeField] private Sprite icon;

    [SerializeField] public string sceneName;
    [SerializeField] public int levelNumber;
    [SerializeField] public bool isUnlocked;
    [SerializeField] public bool isCompleted;

    [Header("Level achievements")]
    [SerializeField] public int scoreToUnlock;
    [SerializeField] public int scoreToComplete;
    [SerializeField] public int scoreToWin;
    [SerializeField] public int bestScore;


    [Header("Level Environment")]
    [SerializeField] public ChunkSettings chunkSettings;
    [SerializeField] public int seed;
    [SerializeField] public int activeChunksAmount;
    [SerializeField] public GameObject proceduralChunkPrefab;
    [SerializeField] public GameObject avalanchePrefab;

    [Header("Movement Settings")]

    [SerializeField] public float gravity = 9.81f;
    [SerializeField] public float baseScrollSpeed;
    [SerializeField] public float scrollSpeedDecreaseRate;
    [SerializeField] public float speedIncreaseDistanceThreshold = 10f;
    [SerializeField] public float speedIncreaseRate = 5f;
    [SerializeField] public float fallThresholdY = -10f;

    [Header("Avalanche Settings")]
    [SerializeField] public int avalancheMaxHits = 3;
    [SerializeField] public float avalancheCloseDuration = 5f;
    [SerializeField] public float avalancheSpawnX = -5f;
    [SerializeField] public float avalancheWobbleFrequency = 0.1f;
    [SerializeField] public float avalancheWobbleAmplitude = 1f;


    [Header("Sounds")]
    [SerializeField] public AudioClip obstacleHitSFX;

    public Sprite GetIcon() => icon;

    public void CheckRecord(float distance)
    {
        if (distance > bestScore)
        {
            bestScore = (int)distance;
        }
    }

    public void Reset()
    {
        bestScore = 0;
        isUnlocked = false;
        isCompleted = false;

    }
}

