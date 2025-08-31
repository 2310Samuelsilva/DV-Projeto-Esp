using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public PlayerTransportData selectedTransport;
    public int totalBalance;

    public int GetTotalBalance() => totalBalance;
    public int AddTotalBalance(int amount) => totalBalance += amount;
}