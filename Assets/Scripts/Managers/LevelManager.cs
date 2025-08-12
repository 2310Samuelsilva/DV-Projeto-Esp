using UnityEngine;

[ExecuteAlways] // So it runs in the editor
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

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            PreviewLevel();
        }
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            LoadLevel();
        }
    }

    private void PreviewLevel()
    {
        ClearPreview();
        if (levelData == null) return;

        // Spawn world manager for preview
        GameObject wm = Instantiate(worldManagerPrefab, Vector3.zero, Quaternion.identity, transform);
        wm.name = "[Preview] WorldManager";
        worldManager = wm.GetComponent<WorldManager>();
        worldManager.Initialize(levelData);
    }

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
        var previews = GetComponentsInChildren<WorldManager>(true);
        foreach (var p in previews)
        {
            if (Application.isPlaying)
                Destroy(p.gameObject);
            else
                DestroyImmediate(p.gameObject);
        }
    }
}