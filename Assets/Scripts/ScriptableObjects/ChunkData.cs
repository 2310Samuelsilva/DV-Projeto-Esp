using UnityEngine;

[CreateAssetMenu(fileName = "ChunkData", menuName = "Level/ChunkData")]
public class ChunkSettings : ScriptableObject
{
    [Header("Terrain Settings")]
    public int resolution = 20;     // Number of segments
    public float width = 10f;       // Total width in world units
    public float baseHeight = 0f;   // Baseline Y
    public float amplitude = 2f;    // Height variation
    public float noiseScale = 0.2f; // Controls noise frequency
    public int seed = 0;            // Random seed
    
}