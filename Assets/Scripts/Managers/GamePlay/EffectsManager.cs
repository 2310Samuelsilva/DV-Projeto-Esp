using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }
    [SerializeField] private float amplitude = 4f;
    [SerializeField] private float frequency = 4f;


    [SerializeField] private GameObject obstacleHitVFXPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        CameraManager.Instance.ShakeCamera(amplitude, frequency, duration);
    }

    public void ObstacleHit(Vector3 position)
    {
        // Camera shake
        //ShakeCamera(amplitude, frequency, 0.25f);

        // VFX
        if (obstacleHitVFXPrefab != null) { }
        ;
        //Instantiate(obstacleHitVFXPrefab, position, Quaternion.identity);

        // TODO: add SFX if you want
    }
}