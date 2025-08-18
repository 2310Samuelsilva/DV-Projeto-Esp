using UnityEngine;

[CreateAssetMenu(fileName = "ChunkData", menuName = "Level/ChunkSettings")]
public class ChunkSettings : ScriptableObject
{
    [Header("Terrain Settings")]
    public int resolution = 20;     // Number of points
    public float width = 10f;       // Total width in world units
    public float baseHeight = 0f;   // Baseline Y
    public float amplitude = 2f;    // Height variation
    public float noiseScale = 0.2f; // Controls noise frequency
    public int seed = 0;            // Random seed


    [Header("Obstacle Settings")]
    public ObstacleSettings[] obstaclePool;

    [Range(0, 1)]
    public float obstacleSpawnChance = 0.5f;

    
    public int obstacleFrequency = 5; // In Chunk points
    



// Enable preview in unity editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Find all WorldManagers in the scene and refresh them
        var managers = FindObjectsByType<WorldManager>(FindObjectsSortMode.None);
        foreach (var manager in managers)
        {
            manager.Reset();
        }
    }
    #endif
    
}