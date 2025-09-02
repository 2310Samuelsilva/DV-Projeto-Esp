using UnityEngine;

public class AvalancheController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData levelData;          // Holds base speeds, thresholds, scales
    [SerializeField] private Transform snowParticles;      // Optional visual particles

    [Header("Catch-up Settings")]
    [SerializeField] private float catchUpStrength = 2f;  // How fast avalanche reacts
    [SerializeField] private float maxCatchUpDistance = 20f; // Distance where it hits max speed

    private float currentX;
    private float baseY;
    private float nextSpeedThreshold;
    private float currentBaseSpeed;

    /// <summary>
    /// Initialize avalanche
    /// </summary>
    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;
        baseY = transform.position.y;

        currentBaseSpeed = levelData.avalancheBaseSpeed;
        nextSpeedThreshold = levelData.avalancheDistanceStep;

        Reset();
    }

    /// <summary>Reset avalanche to starting position</summary>
    public void Reset()
    {
        if (levelData == null) return;

        currentX = levelData.avalancheSpawnX; // Usually behind player
        UpdateAvalanchePosition();
    }

    /// <summary>Update avalanche based on world scroll speed and distance travelled</summary>
    public void UpdateAvalanche(float worldScrollSpeed, float distanceTraveled)
    {
        if (levelData == null) return;

        // ---- Threshold speed increase ----
        if (distanceTraveled >= nextSpeedThreshold)
        {
            currentBaseSpeed += levelData.avalancheSpeedStep;
            nextSpeedThreshold += levelData.avalancheDistanceStep;
        }

        // ---- Distance to player (X = 0) ----
        float distanceToPlayer = -currentX; // Player is at X = 0

        // ---- Catch-up speed ----
        float catchUpFactor = Mathf.Clamp01(distanceToPlayer / maxCatchUpDistance);
        float targetSpeed = currentBaseSpeed + worldScrollSpeed * catchUpFactor;

        // Smoothly move avalanche forward
        currentX += Mathf.Lerp(0f, targetSpeed, catchUpStrength * Time.deltaTime);

        UpdateAvalanchePosition();
        //UpdateSnowParticles(distanceToPlayer);
    }

    private void UpdateAvalanchePosition()
    {
        // Wobble for realism
        float wobble = Mathf.Sin(Time.time * levelData.avalancheWobbleFrequency) * levelData.avalancheWobbleAmplitude;
        transform.position = new Vector3(currentX, baseY + wobble, transform.position.z);
    }

    // private void UpdateSnowParticles(float distanceToPlayer)
    // {
    //     if (snowParticles == null) return;

    //     // Static scale settings
    //     float minScale = 1f;
    //     float maxScale = 5f;
    //     float breathingAmplitude = 0.1f;
    //     float breathingFrequency = 1.5f;

    //     // Scale avalanche visually based on distance to player
    //     float t = Mathf.Clamp01(distanceToPlayer / 2f); // distance normalization
    //     t = Mathf.Pow(1f - t, 2f); // exponential falloff

    //     float scale = Mathf.Lerp(minScale, maxScale, t);
    //     scale += Mathf.Sin(Time.time * breathingFrequency) * breathingAmplitude;

    //     snowParticles.localScale = Vector3.one * scale;

    //     // Position particles slightly behind avalanche
    //     snowParticles.position = new Vector3(currentX - 1f, transform.position.y, snowParticles.position.z);
    // }
    /// <summary>Returns current avalanche X position</summary>
    public float GetAvalanchePosition() => currentX;
}