using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Components;

// private string GetLocalizedString(string key)
// {
//     var table = LocalizationSettings.StringDatabase.GetTable("UI_Texts");
//     if (table != null && table.TryGetEntry(key, out var entry))
//     {
//         return entry.GetLocalizedString();
//     }
//     else
//     {
//         Debug.LogWarning("Missing localized string: " + key);
//         return key; // fallback
//     }
// }

public class LevelSelectPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private Image levelImage;

    [SerializeField] private Button buttonSelect;   // Select button

    [Header("Data")]
    private LevelData levelData;
    private PlayerData playerData;

    public void Setup(LevelData levelData)
    {
        this.levelData = levelData;

        buttonSelect.onClick.RemoveAllListeners();
        buttonSelect.onClick.AddListener(OnSelectButtonPressed);

        RefreshUI();
    }

    public void RefreshUI()
    {
        titleText.text = levelData.levelName.ToUpper();
        levelImage.sprite = levelData.GetIcon();
        if (!levelData.isUnlocked)
        {
            // Locked state
            recordText.text = "Unlock at: " + levelData.scoreToUnlock.ToString() + "m";
            SetButtonState(buttonSelect, false, ("menu.locked"));

        }
        else
        {
            // Unlocked state


            recordText.text = $"{levelData.bestScore}m\n";

            // Handle Upgrade Button
            //SetButtonState(buttonSelect, true, GetLocalizedString("menu.play"));
            SetButtonState(buttonSelect, true, "menu.play");
        }
    }

    private void SetButtonState(Button button, bool interactable, string key = null)
    {
        button.interactable = interactable;

        if (!string.IsNullOrEmpty(key))
        {
            var text = LocalizationSettings.StringDatabase.GetLocalizedString("Game", key);
            button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

        // Dim button visually when not interactable
        var canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = interactable ? 1f : 0.5f;
    }


    private void OnSelectButtonPressed()
    {
        GameManager.Instance.LoadLevel(levelData);
        //GameManager.Instance.LoadSelectedLevel();

    }
}