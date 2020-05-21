using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public GameObject LobbyEntriesPanel;
    public GameObject LobbyEntryPrefab;

    public void AddLobbyEntry (ClientConnectedPlayer player) {
        LobbyEntry lobbyEntry = Instantiate(LobbyEntryPrefab, LobbyEntriesPanel.transform).GetComponent<LobbyEntry>();
        lobbyEntry.Username = player.Username;
        player.PlayerPingUpdated += lobbyEntry.OnConnectedPlayerPingUpdated;
        player.PlayerReady += lobbyEntry.OnConnectedPlayerReadyUpdated;
        player.PlayerDisconnected += lobbyEntry.OnPlayerDisconnected;
    }
    public void AddLobbyEntry (Client client) {
        LobbyEntry scoreboardEntry = Instantiate(LobbyEntryPrefab, LobbyEntriesPanel.transform).GetComponent<LobbyEntry>();
        scoreboardEntry.Username = client.Username;
        client.PlayerLatencyUpdated += scoreboardEntry.OnClientLatencyUpdated;
        client.PlayerReadyUpdated += scoreboardEntry.OnClientReadyUpdated;
        client.PlayerDisconnected += scoreboardEntry.OnPlayerDisconnected;
    }
}
