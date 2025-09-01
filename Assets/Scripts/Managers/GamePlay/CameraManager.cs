using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private CinemachineBasicMultiChannelPerlin noise; // for shake

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (cinemachineCamera == null)
            cinemachineCamera = GetComponent<CinemachineCamera>();

        if (cinemachineCamera != null)
            noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void SetTarget(Transform playerTransform)
    {
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = playerTransform;
            cinemachineCamera.LookAt = playerTransform;
            Debug.Log("Cinemachine target set to player");
        }
        else
        {
            Debug.LogError("No Virtual Camera assigned in CameraManager!");
        }
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {

        if (noise == null)
        { 
            Debug.LogError("No Noise assigned in CameraManager!");
        }
        
        StartCoroutine(DoShake(amplitude, frequency, duration));
    }

    private System.Collections.IEnumerator DoShake(float amplitude, float frequency, float duration)
    {
        Debug.Log("Shaking camera");
        float originalAmplitude = noise.AmplitudeGain;
        float originalFrequency = noise.FrequencyGain;

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to normal
        noise.AmplitudeGain = originalAmplitude;
        noise.FrequencyGain = originalFrequency;
    }
}