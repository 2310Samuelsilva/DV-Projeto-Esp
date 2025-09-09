using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "Game/LevelList")]
public class LevelList : ScriptableObject
{
    [Header("Levels")]
    [SerializeField] public List<LevelData> levels;
    [SerializeField] public LevelData selectedLevel;

    void Awake()
    {
        if (selectedLevel == null) { selectedLevel = levels[0]; }
    }


    
    public bool LevelExists(LevelData levelData)
    {
        return levels.Contains(levelData);
    }

    public LevelData GetLevelData(string levelName)
    {
        return levels.Find(level => level.name == levelName);
    }

    public void SetSelectedLevel(LevelData levelData)
    {
        selectedLevel = levelData;
    }

    public void CheckUnlockLevel(float distance)
    {
        // check if there is a next level
        if (levels.IndexOf(selectedLevel) + 1 >= levels.Count) return;
        
        LevelData nextLevel = levels[levels.IndexOf(selectedLevel) + 1];
        if (nextLevel != null && !nextLevel.isUnlocked && distance >= nextLevel.scoreToUnlock)
        {
            nextLevel.isUnlocked = true;
        }
    }

    public LevelData GetSelectedLevel() => selectedLevel;
    public void Reset()
    {
        foreach (var level in levels)
            level.Reset();
    }
}