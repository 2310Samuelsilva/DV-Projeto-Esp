using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    public LevelData levelData;
    public TransportData transportData;

    [Header("Scene References")]
    public Transform playerSpawnPoint;

    [SerializeField] private GameObject worldManagerPrefab;

    private GameObject playerInstance;
    private WorldManager worldManager;

    // Track the last data used for preview so we only refresh when needed
    private LevelData lastPreviewData;

//     private void OnValidate()
//     {
// #if UNITY_EDITOR
//         if (!Application.isPlaying && levelData != null && transportData != null)
//         {
//             // Delete all children of this component
//             foreach (Transform child in transform)
//             {
//                 if (Application.isPlaying)
//                     Destroy(child.gameObject);
//                 else
//                     DestroyImmediate(child.gameObject);
//             }
//         }
// #endif
//     }

    private void Start()
    {
        if (Application.isPlaying)
        {
            LoadLevel();
        }
    }

    // public void PreviewLevel()
    // {
    //     ClearPreview();

    //     if (levelData == null) return;

    //     // Spawn world manager for preview
    //     GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity, transform);
    //     wm.name = "[Preview] WorldManager";
    //     worldManager = wm.GetComponent<WorldManager>();
    //     worldManager.Initialize(levelData);
    // }

    private void LoadLevel()
    {
        if (levelData == null || transportData == null)
        {
            Debug.LogError("LevelManager: Missing LevelData or TransportData!");
            return;
        }

        playerInstance = Instantiate(transportData.prefab, playerSpawnPoint.position, Quaternion.identity);
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();
        playerController.Initialize(transportData);

        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity);
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData);
    }

    private void ClearPreview()
    {
        var worldManagers = GetComponentsInChildren<WorldManager>(true);
        foreach (var wm in worldManagers)
        {
            DestroyImmediate(wm.gameObject);
        }
        worldManager = null; // Clear reference
    }
}