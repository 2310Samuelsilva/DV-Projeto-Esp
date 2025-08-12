using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class WorldManager : MonoBehaviour
{
    [SerializeField] private GameObject terrainLoaderPrefab;
    [SerializeField] private LevelData levelData;

    [SerializeField] private float scrollSpeed;
    private float distanceTraveled;

    [SerializeField] private TerrainLoader terrainLoader;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || levelData == null) return;

        // Delay so Unity is not in the middle of rendering
        EditorApplication.delayCall += () =>
        {
            if (this == null) return; // Object might be gone
            Initialize(levelData);
        };

    }
#endif

    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;

        if (terrainLoader != null)
        {
            if (Application.isPlaying)
                Destroy(terrainLoader.gameObject);
            else
                DestroyImmediate(terrainLoader.gameObject);
        }

        GameObject tl = Instantiate(terrainLoaderPrefab, Vector3.zero, Quaternion.identity, transform);
        terrainLoader = tl.GetComponent<TerrainLoader>();
        terrainLoader.Initialize(levelData);

        Reset();
        

    }

    public void Reset()
    {
        distanceTraveled = 0f;
        scrollSpeed = levelData != null ? levelData.baseScrollSpeed : 0f;
        if (terrainLoader != null)
            terrainLoader.InitializeChunks();
    }

    private void Update()
    {
        if (Application.isPlaying && terrainLoader != null)
        {
            distanceTraveled += scrollSpeed * Time.deltaTime;
            terrainLoader.MoveChunks(scrollSpeed);
        }
    }

    public void IncreaseScrollSpeed()
    {
        scrollSpeed += levelData.speedIncreaseRate * Time.deltaTime;
    }

    public float DistanceTravelled() => distanceTraveled;
}