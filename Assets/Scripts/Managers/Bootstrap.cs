using System.Collections;
using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Wait one frame to ensure all Awake() methods of managers run
        yield return null;

        // Ensure all required managers exist
        if (GameManager.Instance == null)
            Debug.LogError("GameManager not found!");

        if (SceneLoader.Instance == null)
            Debug.LogError("SceneLoader not found!");

        // Entry point -> Load Menu
        GameManager.Instance.ReturnToMainMenu();
    }
}