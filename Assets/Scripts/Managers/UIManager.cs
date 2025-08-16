// using UnityEngine;

// public class UIManager : MonoBehaviour
// {
//     public static UIManager Instance { get; private set; }

//     [Header("Panels")]
//     [SerializeField] private GameObject startPanel;
//     [SerializeField] private GameObject gameOverPanel;

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         Instance = this;
//     }

//     public void ShowStartPanel()
//     {
//         // if (startPanel != null)
//         //     startPanel.SetActive(true);
//     }

//     public void HideStartPanel()
//     {
//         // if (startPanel != null)
//         //     startPanel.SetActive(false);
//     }

//     public void ShowGameOverPanel()
//     {
//         // if (gameOverPanel != null)
//         //     gameOverPanel.SetActive(true);
//     }

//     public void HideGameOverPanel()
//     {
//         // if (gameOverPanel != null)
//         //     gameOverPanel.SetActive(false);
//     }

//     public void UpdateLevelUI(int level)
//     {
//         UIGameplayManager.Instance.UpdateLevelUI(level);
//     }
    
//     // public void UpdateChargeBar(float currentForce, float maxForce)
//     // {
//     //     UIGameplayManager.Instance.UpdateChargeUI(currentForce, maxForce);    
//     // }
    
// }