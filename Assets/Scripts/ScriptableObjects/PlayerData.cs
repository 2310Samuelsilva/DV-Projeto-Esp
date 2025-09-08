using UnityEngine;

/*
PlayerData is a scriptable object that holds the data for the player.
*/
[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string id;
    public string Id => id;

    public PlayerTransportData selectedTransport;
    public int totalBalance;

    public int GetTotalBalance() => totalBalance;
    public int AddTotalBalance(int amount) => totalBalance += amount;
}