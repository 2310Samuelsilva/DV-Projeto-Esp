using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        //GameManager.Instance.LoadLevel(0); // Load first level
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game"); // Won’t quit in Editor
        Application.Quit();
    }
}