using UnityEngine;

public class AvalancheController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform snowParticles;

    [Header("Threshold-Based Speed")]
    [SerializeField] private float currentSpeed;       // Current avalanche speed
    private float nextSpeedThreshold;                  // Distance at which next speed increase triggers

    private float currentX;
    private float baseY;

    /// <summary>
    /// Initialize avalanche
    /// </summary>
    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;
        baseY = transform.position.y;
        Reset();
    }

    /// <summary>Reset avalanche to starting position and thresholds</summary>
    public void Reset()
    {
        if (levelData == null) return;

        currentX = levelData.avalancheSpawnX; // usually behind player
        currentSpeed = levelData.avalancheBaseSpeed;
        nextSpeedThreshold = levelData.avalancheDistanceStep; // first threshold
        UpdateAvalanchePosition();
    }

    /// <summary>Update avalanche position based on distance traveled by player/world</summary>
    public void UpdateAvalanche(float distanceTraveled)
    {
        if (levelData == null) return;

        // ---- THRESHOLD SPEED INCREASE ----
        if (distanceTraveled >= nextSpeedThreshold)
        {
            currentSpeed += levelData.avalancheSpeedStep;    // Increment speed
            nextSpeedThreshold += levelData.avalancheDistanceStep; // Move to next threshold
            Debug.Log($"Avalanche speed increased to {currentSpeed}, next threshold at {nextSpeedThreshold}");
        }

        // Move avalanche forward
        currentX += currentSpeed * Time.deltaTime;
        UpdateAvalanchePosition();
    }

    private void UpdateAvalanchePosition()
    {
        // Add wobble for realism
        float wobble = Mathf.Sin(Time.time * levelData.avalancheWobbleFrequency) * levelData.avalancheWobbleAmplitude;
        transform.position = new Vector3(currentX, baseY + wobble, transform.position.z);
    }

    public float GetAvalanchePosition() => currentX;
}