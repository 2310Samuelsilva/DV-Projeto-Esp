using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTransportShopPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private Button buttonAction;   // Buy/Upgrade button
    [SerializeField] private Button buttonSelect;   // Select button

    [Header("Data")]
    private PlayerTransportData playerTransportData;
    private PlayerData playerData;

    public void Setup(PlayerTransportData playerTransportData)
    {
        this.playerTransportData = playerTransportData;
        this.playerData = LevelManager.Instance.GetPlayerData();

        // Clear previous listeners
        buttonAction.onClick.RemoveAllListeners();
        buttonSelect.onClick.RemoveAllListeners();

        buttonAction.onClick.AddListener(OnActionButtonPressed);
        buttonSelect.onClick.AddListener(OnSelectButtonPressed);

        RefreshUI();
    }

    public void RefreshUI()
    {
        titleText.text = playerTransportData.GetName();

        if (!playerTransportData.IsUnlocked())
        {
            // Locked
            levelText.text = "Locked";
            priceText.text = $"Buy: {playerTransportData.GetBasePrice()}";
            buttonAction.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";

            buttonSelect.interactable = false;
            buttonSelect.GetComponentInChildren<TextMeshProUGUI>().text = "Locked";
        }
        else
        {
            // Unlocked
            levelText.text = $"Level {playerTransportData.GetLevel()}";
            priceText.text = $"Upgrade: {playerTransportData.GetUpgradePrice()}";
            buttonAction.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade";

            if (playerData.selectedTransport == playerTransportData)
            {
                buttonSelect.interactable = false;
                buttonSelect.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
            }
            else
            {
                buttonSelect.interactable = true;
                buttonSelect.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            }
        }
    }

    private void OnActionButtonPressed()
    {
        if (!playerTransportData.IsUnlocked())
        {
            // Buy
            if (playerData.totalBalance >= playerTransportData.GetBasePrice())
            {
                playerData.totalBalance -= playerTransportData.GetBasePrice();
                playerTransportData.Unlock();
                Debug.Log($"Bought {playerTransportData.GetName()}");
            }
            else
            {
                Debug.Log("Not enough currency to buy!");
            }
        }
        else
        {
            // Upgrade
            int upgradePrice = playerTransportData.GetUpgradePrice();
            if (playerData.totalBalance >= upgradePrice)
            {
                playerData.totalBalance -= upgradePrice;
                playerTransportData.IncreaseLevel();
                Debug.Log($"Upgraded {playerTransportData.GetName()} to level {playerTransportData.GetLevel()}");
            }
            else
            {
                Debug.Log("Not enough currency to upgrade!");
            }
        }

        RefreshUI();
    }

    private void OnSelectButtonPressed()
    {
        playerData.selectedTransport = playerTransportData;
        Debug.Log($"{playerTransportData.GetName()} selected!");

        // Refresh all panels so only one is selected
        UIShopManager.Instance.RefreshAllPanels();
    }
}