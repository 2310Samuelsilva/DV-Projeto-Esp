using UnityEngine;

[CreateAssetMenu(fileName = "TransportData", menuName = "Game/Transport Data")]
public class TransportData : ScriptableObject {
    public string transportName;
    public GameObject prefab;
    public float speedMultiplier;
    public float jumpForce;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float handling; // could be used for obstacle dodge responsiveness
}