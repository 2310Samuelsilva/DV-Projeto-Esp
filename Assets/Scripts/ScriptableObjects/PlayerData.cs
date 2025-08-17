using UnityEngine;

[CreateAssetMenu(fileName = "TransportData", menuName = "Game/Transport Data")]
public class PlayerData : ScriptableObject
{
    public PlayerTransportData selectedTransport;
    public float totalBalance;
}