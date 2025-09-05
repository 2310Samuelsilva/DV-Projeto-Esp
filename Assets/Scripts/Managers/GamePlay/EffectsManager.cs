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
    
    public void ShakeCamera(string shakeId, float amplitude, float frequency, float duration)
    {
        CameraManager.Instance.ShakeCamera(shakeId, amplitude, frequency, duration);
    }

    public void ObstacleHit(Vector3 position)
    {
        


        // VFX
        if (obstacleHitVFXPrefab != null)
        {
            GameObject vfxInstance = Instantiate(obstacleHitVFXPrefab, position, Quaternion.identity);
            Destroy(vfxInstance, 0.5f);
        }
        
        ShakeCamera("obstacle", amplitude, frequency, 0.25f);
        

        //Instantiate(obstacleHitVFXPrefab, position, Quaternion.identity);

        // TODO: add SFX if you want
        AudioManager.Instance.PlaySFX(LevelManager.Instance.GetLevelData().obstacleHitSFX, 0.5f);
    }
}