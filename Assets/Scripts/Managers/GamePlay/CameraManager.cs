using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;



public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private PolygonCollider2D confinerCollider;
    private CinemachineBasicMultiChannelPerlin noise;
    private CinemachineConfiner2D confiner;

    private class ShakeRequest
    {
        public float amplitude;
        public float frequency;
        public float timeLeft;

        public ShakeRequest(float amplitude, float frequency, float timeLeft)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.timeLeft = timeLeft;
        }
    }

    private readonly Dictionary<string, ShakeRequest> activeShakes = new Dictionary<string, ShakeRequest>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (cinemachineCamera == null) cinemachineCamera = Camera.main.GetComponent<CinemachineCamera>();
        noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        // if (confiner != null)
        // {
        //     confiner.BoundingShape2D = confinerCollider;
        // }
    }

    public void SetTarget(Transform target)
    {
        cinemachineCamera.Follow = target;
        cinemachineCamera.LookAt = target;
        
        // if (confiner != null)
        //     confiner.InvalidateBoundingShapeCache();
    }

    public void ShakeCamera(string shakeId, float amplitude, float frequency, float timeLeft)
    {

        if (activeShakes.ContainsKey(shakeId))
        {
            activeShakes[shakeId].amplitude = amplitude;
            activeShakes[shakeId].frequency = frequency;
            activeShakes[shakeId].timeLeft = timeLeft;
            return;
        }

        activeShakes[shakeId] = new ShakeRequest(amplitude, frequency, timeLeft);
    }


    private void Update()
    {

        if(LevelManager.Instance.IsPaused())
        {
            return;
        }

        if (activeShakes.Count == 0)
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
            //Debug.Log("No active shakes");
            return;
        }

        //Debug.Log($"Active shakes: {activeShakes.Count}");

        float totalAmplitude = 0f;
        float totalFrequency = 0f;

        // Update active shakes
        List<string> expiredKeys = new List<string>();
        foreach (KeyValuePair<string, ShakeRequest> kvp in activeShakes)
        {
            ShakeRequest shake = kvp.Value;
            totalAmplitude += shake.amplitude;
            totalFrequency += shake.frequency;

            shake.timeLeft -= Time.deltaTime;
            if (shake.timeLeft <= 0f)
                expiredKeys.Add(kvp.Key);
        }

        foreach (string key in expiredKeys)
        {
            activeShakes.Remove(key);
        }

        // Apply combined effect
        noise.AmplitudeGain = totalAmplitude;
        noise.FrequencyGain = totalFrequency;
    }
}