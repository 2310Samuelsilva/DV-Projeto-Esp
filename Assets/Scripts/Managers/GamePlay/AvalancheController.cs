using UnityEngine;

public class AvalancheController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private Transform snowParticles;

    [Header("Scaling & Effects")]
    [SerializeField] private float baseScale = 0.2f;
    [SerializeField] private float scalePerHit = 0.05f;
    [SerializeField] private float maxScale = 1.5f;

    [Header("Particles")]
    [SerializeField] private int baseEmissionRate = 200;
    [SerializeField] private int emissionPerHit = 50;
    [SerializeField] private int maxEmissionRate = 400;

    [Header("Movement")]
    [SerializeField] private float moveLerpSpeed = 3f;   // Smoothness of catch-up
    [SerializeField] private float wobbleAmplitude = 0.2f;
    [SerializeField] private float wobbleFrequency = 2f;

    private float currentX;
    private float targetX;
    private float baseY;
    private Vector3 playerPosition;

    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;
        this.playerPosition = LevelManager.Instance.GetPlayerSpawnPoint();
        baseY = transform.position.y;
        Reset();
    }

    public void Reset()
    {
        if (levelData == null) return;

        currentX = levelData.avalancheSpawnX; // Start behind player
        targetX = currentX;
        SetClose(0);
        UpdateAvalanchePosition();
    }

    /// <summary>
    /// Set how "close" the avalanche appears based on hit count
    /// </summary>
    public void SetClose(int hitCount)
    {
        // --- Scale ---
        float scale = baseScale + hitCount * scalePerHit;
        scale = Mathf.Min(scale, maxScale);
        transform.localScale = Vector3.one * scale;

        if (snowParticles != null)
            snowParticles.localScale = Vector3.one * scale;

        if (particleSystem != null)
        {
            particleSystem.transform.localScale = Vector3.one * scale;

            var emission = particleSystem.emission;
            int rate = baseEmissionRate + hitCount * emissionPerHit;
            rate = Mathf.Min(rate, maxEmissionRate);
            emission.rateOverTime = rate;
        }

        // --- Set new target position closer to player ---
        float cameraLeftEdge = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);
        targetX = cameraLeftEdge + 0.5f; // 0.5f offset from camera
    }

    public void UpdateAvalanche(float worldScrollSpeed, float distanceTraveled)
    {
        // Smoothly move currentX toward targetX
        currentX = Mathf.Lerp(currentX, targetX, moveLerpSpeed * Time.deltaTime);
        UpdateAvalanchePosition();
    }

    private void UpdateAvalanchePosition()
    {
        float wobble = Mathf.Sin(Time.time * wobbleFrequency) * wobbleAmplitude;
        transform.position = new Vector3(currentX, baseY + wobble, transform.position.z);
    }

    public float GetAvalanchePosition() => currentX;
}