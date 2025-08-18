using UnityEngine;
using System.Collections.Generic;

public class UIShopManager : MonoBehaviour
{
    public static UIShopManager Instance { get; private set; }

    [SerializeField] private GameObject transportPanelPrefab;
    [SerializeField] private Transform transportListContainer;
    [SerializeField] private PlayerTransportDatabase playerTransportDatabases;

    private readonly List<PlayerTransportShopPanel> panels = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
            
        }
        Instance = this;
    }

    void Start()
    {
        PopulateShop();
    }

    public void PopulateShop()
    {
        foreach (Transform child in transportListContainer)
            Destroy(child.gameObject); // Clear old panels

        panels.Clear();

        foreach (var transport in playerTransportDatabases.transports)
        {
            GameObject panelObj = Instantiate(transportPanelPrefab, transportListContainer);
            var panel = panelObj.GetComponent<PlayerTransportShopPanel>();
            panel.Setup(transport);
            panels.Add(panel);
        }
    }

    public void RefreshAllPanels()
    {
        foreach (var panel in panels)
            panel.RefreshUI(); // Call UI refresh
    }
}