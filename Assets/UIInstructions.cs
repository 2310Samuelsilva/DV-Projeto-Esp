using UnityEngine;

public class UIInstructions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
