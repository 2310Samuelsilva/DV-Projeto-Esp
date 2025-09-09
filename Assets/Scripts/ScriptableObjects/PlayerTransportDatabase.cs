using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTransportDatabase", menuName = "Game/Transport Database")]
public class PlayerTransportDatabase : ScriptableObject
{
    public List<PlayerTransportData> transports;

    public void Reset()
    {
        foreach (var transport in transports)
            transport.Reset();
    }

    public PlayerTransportData GetSelectedTransport()
    {
        return transports.Find(t => t.IsSelected());
    }
}