using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            SetButtonState(buttonSelect, false, "Locked");

        }
        else
        {
            // Unlocked state
            

            recordText.text = $"Record: {levelData.bestScore}m\n";

            // Handle Upgrade Button
            SetButtonState(buttonSelect, true, "Play");
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


    private void OnSelectButtonPressed()
    {
        GameManager.Instance.LoadLevel(levelData);
        //GameManager.Instance.LoadSelectedLevel();

    }
}