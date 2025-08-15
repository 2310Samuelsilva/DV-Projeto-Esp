using UnityEngine;

public class AvalancheController : MonoBehaviour
{
    [Header("Avalanche Settings")]
    [SerializeField] private float wobbleAmplitude = 0.2f;     // Up/down wobble amount
    [SerializeField] private float wobbleFrequency = 2f;       // Wobble speed

    [Header("Speed & Catch-up")]
    [SerializeField] private float maxCatchUpBonus = 0.4f;       // Max speed increase over base
    [SerializeField] private float catchUpStrength = 0.15f;    // How quickly avalanche reacts
    [SerializeField] private float growthOverTime = 0.002f;     // Constant ramp each second

    [Header("Scale & Visuals")]
    [SerializeField] private float maxScale = 3f;              // Avalanche size at player's back
    [SerializeField] private float minScale = 1f;              // Size when far away
    [SerializeField] private float maxDistance = 5f;          // Distance for full scale
    [SerializeField] private float breathingAmplitude = 0.05f; // Scale pulse amount
    [SerializeField] private float breathingFrequency = 1.5f;  // Scale pulse speed

    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform snowParticles;

    private float avalancheX;
    private float currentSpeed;
    private float baseY;

    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;
        Reset();
    }

    public void Reset()
    {
        avalancheX = levelData.avalancheSpawnX; // Start position behind player
        currentSpeed = levelData.avalancheSpeed;
        baseY = transform.position.y;
        UpdateAvalanchePosition();
    }

    public void UpdateAvalanche(float scrollSpeed)
    {
        float distanceToPlayer = -avalancheX; // Player X = 0

        // ---- SPEED LOGIC ----
        float baseSpeed = levelData.avalancheSpeed;

        // Bonus speed grows slower for large distances
        float catchUpBonus = Mathf.Min(maxCatchUpBonus, Mathf.Log(distanceToPlayer + 1f) * 1.2f);

        // Gradual increase over time for late-game difficulty
        baseSpeed += Time.timeSinceLevelLoad * growthOverTime;

        float targetSpeed = baseSpeed;

        // Smooth acceleration toward target speed
        //currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * catchUpStrength);
        currentSpeed = targetSpeed * Time.fixedDeltaTime;
        // Move avalanche forward
        avalancheX += currentSpeed * Time.deltaTime;

        UpdateAvalanchePosition();
        UpdateSnowParticles(scrollSpeed);
    }

    private void UpdateAvalanchePosition()
    {
        // Add subtle vertical wobble for realism
        float wobble = Mathf.Sin(Time.time * wobbleFrequency) * wobbleAmplitude;
        transform.position = new Vector3(avalancheX, baseY + wobble, transform.position.z);
    }

    private void UpdateSnowParticles(float terrainSpeed)
    {
        if (snowParticles == null) return;

        float distanceToPlayer = -avalancheX;

        // Position snow behind avalanche core
        snowParticles.position = new Vector3(avalancheX - 1f, transform.position.y, snowParticles.position.z);

        // Particle velocity matches terrain + avalanche
        var vel = snowParticles.GetComponent<ParticleSystem>().velocityOverLifetime;
        vel.x = currentSpeed;

        // ---- SCALE LOGIC ----
        // Non-linear scaling: closer → much bigger
        float t = Mathf.Clamp01(distanceToPlayer / maxDistance);
        t = Mathf.Pow(1f - t, 2f); // exponential curve
        float scale = Mathf.Lerp(minScale, maxScale, t);

        // Breathing pulse effect
        scale += Mathf.Sin(Time.time * breathingFrequency) * breathingAmplitude;

        snowParticles.localScale = Vector3.one * scale;
    }

    public float GetAvalanchePosition() => avalancheX;
}