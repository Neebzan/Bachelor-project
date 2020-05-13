﻿using DatabaseREST.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        //Check if spawning local player or remote player
        //if (_id == Client.instance.id)
        //    _player = Instantiate(localPlayerPrefab, _position, _rotation);
        //else

        _player = Instantiate(playerPrefab, _position, _rotation);

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        //_player.GetComponent<PlayerManager>().username = _username;

        players.Add(_id, _player.GetComponent<PlayerManager>());

    }
}
