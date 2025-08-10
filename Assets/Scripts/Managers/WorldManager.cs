
using System.Runtime.CompilerServices;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    [SerializeField] private GameObject terrainLoaderPrefab;
    private  LevelData levelData;  // Assigned by LevelManager

    [SerializeField] private float scrollSpeed;
    private float distanceTraveled;

    private TerrainLoader terrainLoader;



    public void Initialize(LevelData levelData)
    {
        Debug.Log("WorldManager: Initialize");
        this.levelData = levelData;
        GameObject tl = Instantiate(terrainLoaderPrefab, Vector3.zero, Quaternion.identity);
        terrainLoader = tl.GetComponent<TerrainLoader>();
        terrainLoader.Initialize(levelData);
        Reset();
    }

    void Reset()
    {
        distanceTraveled = 0f;
        scrollSpeed = levelData.baseScrollSpeed;
        terrainLoader.InitializeChunks();
    }

    void Update()
    {
        Debug.Log("WorldManager: Update. ScrollSpeed: " + scrollSpeed);
        distanceTraveled += scrollSpeed * Time.deltaTime;
        terrainLoader.MoveChunks(scrollSpeed);
    }

    public void SetScrollSpeed(float newSpeed)
    {
        scrollSpeed = newSpeed;
    }
    
    public float DistanceTravelled() { return distanceTraveled; }
}