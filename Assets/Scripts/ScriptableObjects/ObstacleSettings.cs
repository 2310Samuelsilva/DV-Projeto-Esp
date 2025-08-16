using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleSettings", menuName = "Level/ObstacleSettings")]
public class ObstacleSettings : ScriptableObject
{
    [Header("Obstacle Settings")]
    public int maxHeigth = 5;
    public int minHeigth = 5;
    public GameObject obstaclePrefab;

    
}