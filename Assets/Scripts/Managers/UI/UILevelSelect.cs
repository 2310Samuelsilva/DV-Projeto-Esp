using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UILevelSelect : MonoBehaviour
{
    public static UILevelSelect Instance { get; private set; }

    [SerializeField] private GameObject levelSelectPrefab;
    [SerializeField] private Transform levelListContainer;
    [SerializeField] private Image levelIcon;
    [SerializeField] private LevelList levelList;


    private readonly List<LevelSelectPanel> panels = new();

    private void Awake()
    {
        Debug.Log("UILevelSelect: Awake");
        if (Instance != null && Instance != this)
        {
            // Debug.Log("Duplicate UIShopManager destroyed!");
            // Destroy(gameObject);
            return;

        }
        Instance = this;
    }

    void Start()
    {   
        this.levelList = GameManager.Instance.GetLevelList();

        Populate();
    }

    public void Populate()
    {
        foreach (Transform child in levelListContainer)
            Destroy(child.gameObject); // Clear old panels


        panels.Clear();

        foreach (var level in levelList.levels)
        {
            //Debug.Log($"Creating panel for {level.levelName}");
            GameObject panelObj = Instantiate(levelSelectPrefab, levelListContainer);
            var panel = panelObj.GetComponent<LevelSelectPanel>();
            panel.Setup(level);
            panels.Add(panel);
        }
    }

    public void RefreshAllPanels()
    {

        foreach (var panel in panels)
            panel.RefreshUI(); // Call UI refresh
    }
    
    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}