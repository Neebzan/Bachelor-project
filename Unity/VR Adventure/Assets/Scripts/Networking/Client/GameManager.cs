using DatabaseREST.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Dictionary<int, ClientConnectedPlayer> EmulatedPlayers = new Dictionary<int, ClientConnectedPlayer>();
    public Dictionary<int, EmulatedFireball> EmulatedFireballs = new Dictionary<int, EmulatedFireball>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject EmulatedFireballPrefab;

    public ScoreboardUI ScoreboardUI;

    private void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }

    public void SpawnPlayer (int _id, string _username, Vector3 _position, Quaternion _rotation) {
        GameObject _player;
        _player = Instantiate(playerPrefab, _position, _rotation);

        ClientConnectedPlayer clientConnectedPlayer = _player.GetComponent<ClientConnectedPlayer>();
        clientConnectedPlayer.Initialize(_id, _username);

        EmulatedPlayers.Add(_id, _player.GetComponent<ClientConnectedPlayer>());

        ScoreboardUI.AddScoreboardEntry(clientConnectedPlayer);
    }

    public void SpawnEmulatedFireball (int id, Vector3 position, float size) {
        EmulatedFireball emulatedFireball = Instantiate(EmulatedFireballPrefab, position, Quaternion.identity).GetComponent<EmulatedFireball>();
        emulatedFireball.Init(id, size);
        EmulatedFireballs.Add(id, emulatedFireball);
    }
}
