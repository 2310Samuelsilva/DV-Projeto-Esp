using TMPro;
using UnityEngine;

public class UILevelEndManager : MonoBehaviour
{
    public static UILevelEndManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI coinsText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log("UILevelEndManager: Awake");

    }

   

    public void PopulateUI(string distance, int coins)
    {
        distanceText.text = distance;
        coinsText.text = coins.ToString();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PlayAgain()
    {
        GameManager.Instance.RestartLevel();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
