using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _pingText;
    [SerializeField]
    private TextMeshProUGUI _usernameText;
    [SerializeField]
    private TextMeshProUGUI _readyText;

    private int ping;
    public int Ping {
        get { return ping; }
        set {
            ping = value;
            _pingText.text = ping.ToString();
        }
    }

    private bool _ready;
    public bool Ready {
        get { return _ready; }
        set {
            _ready = value;
            _readyText.text = _ready ? "√" : "X";
        }
    }


    private string username;

    public string Username {
        get { return username; }
        set {
            username = value;
            _usernameText.text = username;
        }
    }

    public void OnPlayerDisconnected (object sender, EventArgs e) {
        if (sender is ClientConnectedPlayer) {
            ClientConnectedPlayer client = (sender as ClientConnectedPlayer);
            client.PlayerDisconnected -= OnPlayerDisconnected;
            client.PlayerPingUpdated -= OnConnectedPlayerPingUpdated;
            client.PlayerScoreUpdated -= OnConnectedPlayerReadyUpdated;
        }

        else if (sender is Client) {
            Client client = (sender as Client);
            client.PlayerDisconnected -= OnPlayerDisconnected;
            client.PlayerLatencyUpdated -= OnClientLatencyUpdated;
            client.PlayerScoreUpdated -= OnClientReadyUpdated;
        }

        Destroy(this.gameObject);
    }


    // If the player is remote
    public void OnConnectedPlayerReadyUpdated (object sender, EventArgs e) {
        Ready = (sender as ClientConnectedPlayer).Ready;
    }
    public void OnConnectedPlayerPingUpdated (object sender, EventArgs e) {
        Ping = (sender as ClientConnectedPlayer).Ping;
    }

    // If the player  local
    public void OnClientLatencyUpdated () {
        Ping = Client.instance.Latency;
    }
    public void OnClientReadyUpdated () {
        Ready = Client.instance.Ready;
    }
}
