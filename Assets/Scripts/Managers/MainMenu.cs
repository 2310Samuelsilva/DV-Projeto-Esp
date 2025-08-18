using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    private void Start()
    {
        if (GameManager.Instance != null)
            Debug.Log("GameManager is alive!");
        else
            Debug.LogWarning("GameManager missing!");
    }

    public void PlayGame()
    {
        GameManager.Instance.LoadSelectedLevel();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game"); // Won’t quit in Editor
        Application.Quit();
    }
}