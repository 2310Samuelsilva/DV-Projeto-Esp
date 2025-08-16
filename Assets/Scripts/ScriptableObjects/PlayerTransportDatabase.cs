using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTransportDatabase", menuName = "Game/Transport Database")]
public class PlayerTransportDatabase : ScriptableObject
{
    public List<PlayerTransportData> transports;
}