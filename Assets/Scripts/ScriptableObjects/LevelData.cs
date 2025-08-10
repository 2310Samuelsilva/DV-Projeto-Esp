using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject {
    public string levelName;
    public GameObject[] chunkPrefabs;
    public float baseScrollSpeed;
    public float gravity = 9.81f;
    public float speedIncreaseRate;
}