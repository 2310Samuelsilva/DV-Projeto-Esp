using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTransportShopPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button buttonAction;   // Buy/Upgrade button
    [SerializeField] private Button buttonSelect;   // Select button
    [SerializeField] private Slider levelSlider;

    [Header("Data")]
    private PlayerTransportData playerTransportData;
    private PlayerData playerData;

    public void Setup(PlayerTransportData playerTransportData)
    {
        this.playerTransportData = playerTransportData;
        if (this.playerData == null)
        {
            this.playerData = GameManager.Instance.GetPlayerData();
        }

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
            // Locked state
            levelText.text = "Locked";
            SetButtonState(buttonAction, playerData.totalBalance >= playerTransportData.GetBasePrice(), 
                           $"Buy: {playerTransportData.GetBasePrice()}");
            SetButtonState(buttonSelect, false, "Locked");

            if (levelSlider != null)
            {
                levelSlider.gameObject.SetActive(false); // hide slider when locked
            }
        }
        else
        {
            // Unlocked state
            int currentLevel = playerTransportData.GetLevel();
            int maxLevel = playerTransportData.GetMaxPossibleLevel();
            int upgradePrice = playerTransportData.GetUpgradePrice();

            levelText.text = $"Level {currentLevel}";

            // Handle Upgrade Button
            if (currentLevel >= maxLevel)
            {
                SetButtonState(buttonAction, false, "Max Level");
            }
            else
            {
                bool canUpgrade = playerData.totalBalance >= upgradePrice;
                SetButtonState(buttonAction, canUpgrade, $"Upgrade: {upgradePrice}");
            }

            // Handle Select Button
            if (playerData.selectedTransport == playerTransportData)
            {
                SetButtonState(buttonSelect, false, "Selected");
            }
            else
            {
                SetButtonState(buttonSelect, true, "Select");
            }

            // Handle Slider (current level progress to max)
            if (levelSlider != null)
            {
                levelSlider.gameObject.SetActive(true);
                levelSlider.minValue = 0;
                levelSlider.maxValue = maxLevel;
                levelSlider.value = Mathf.Clamp(currentLevel, 0, maxLevel);
            }
        }
    }

    private void SetButtonState(Button button, bool interactable, string overrideText = null)
    {
        button.interactable = interactable;

        if (!string.IsNullOrEmpty(overrideText))
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = overrideText;
        }

        // Dim button visually when not interactable
        var canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = interactable ? 1f : 0.5f;
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
            int maxLevel = playerTransportData.GetMaxPossibleLevel();

            if (playerTransportData.GetLevel() < maxLevel &&
                playerData.totalBalance >= upgradePrice)
            {
                playerData.totalBalance -= upgradePrice;
                playerTransportData.IncreaseLevel();
                Debug.Log($"Upgraded {playerTransportData.GetName()} to level {playerTransportData.GetLevel()}");
            }
            else
            {
                Debug.Log("Cannot upgrade (max level or not enough balance).");
            }
        }

        UITransports.Instance.RefreshAllPanels();
    }

    private void OnSelectButtonPressed()
    {
        playerData.selectedTransport = playerTransportData;
        Debug.Log($"{playerTransportData.GetName()} selected!");

        UITransports.Instance.RefreshAllPanels();
    }
}