using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    [Header("Levels")]

    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerTransportDatabase playerTransportDatabase;
    [SerializeField] private LevelList levelList;

    // Getters & Setters
    public LevelData GetSelectedLevel() => levelList.GetSelectedLevel();
    public PlayerData GetPlayerData() => playerData;
    public PlayerTransportDatabase GetPlayerTransportDatabase() => playerTransportDatabase;
    public LevelList GetLevelList() => levelList;

    public void SetSelectedLevel(LevelData levelData)
    {

        if (levelList.LevelExists(levelData))
        {
            levelList.SetSelectedLevel(levelData);
        }
    }

    public void Reset()
    {
        playerData.Reset();
        playerTransportDatabase.Reset();
        Debug.Log("Selected transport: " + playerTransportDatabase.GetSelectedTransport());
        playerData.selectedTransport = playerTransportDatabase.GetSelectedTransport();
        Debug.Log("Selected transport: " + playerTransportDatabase.GetSelectedTransport());
        levelList.Reset();
    }
}