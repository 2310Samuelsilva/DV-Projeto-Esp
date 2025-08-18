using UnityEngine;

public class PreventDestroy : MonoBehaviour
{
    public static PreventDestroy Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Duplicate GameManager destroyed!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager Awake: instance set, will survive scene loads.");
    }
}
