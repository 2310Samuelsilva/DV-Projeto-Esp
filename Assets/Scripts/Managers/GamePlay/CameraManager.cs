using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
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
}