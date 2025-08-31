using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UITransportManager : MonoBehaviour
{
    public static UITransportManager Instance { get; private set; }

    [SerializeField] private GameObject transportPanelPrefab;
    [SerializeField] private Transform transportListContainer;
    [SerializeField] private PlayerTransportDatabase playerTransportDatabases;

    [SerializeField] private TextMeshProUGUI textBalance;

    private readonly List<PlayerTransportShopPanel> panels = new();
    private PlayerData playerData;

    private void Awake()
    {
        Debug.Log("UIShopManager: Awake");
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
        this.playerData = GameManager.Instance.GetPlayerData();
        PopulateShop();
    }

    public void PopulateShop()
    {
        foreach (Transform child in transportListContainer)
            Destroy(child.gameObject); // Clear old panels

        
        textBalance.text = $"{playerData.GetTotalBalance()}";
        panels.Clear();

        foreach (var transport in playerTransportDatabases.transports)
        {
            Debug.Log($"Creating panel for {transport.GetName()}");
            GameObject panelObj = Instantiate(transportPanelPrefab, transportListContainer);
            var panel = panelObj.GetComponent<PlayerTransportShopPanel>();
            panel.Setup(transport);
            panels.Add(panel);
        }
    }

    public void RefreshAllPanels()
    {

        textBalance.text = $"{playerData.GetTotalBalance()}";
        foreach (var panel in panels)
            panel.RefreshUI(); // Call UI refresh
    }
}