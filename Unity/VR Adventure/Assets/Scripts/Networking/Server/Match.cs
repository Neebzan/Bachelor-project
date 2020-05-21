using DatabaseREST.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Match : MonoBehaviour
{
    Matches _match;
    bool _playersReady = false;
    bool _started = false;
    Stopwatch _sW = new Stopwatch();

    TimeSpan _matchDuration = new TimeSpan(0, 0, 10);
    DateTime _matchStarted;
    DateTime _matchEnded;

    int playerReadyCount = 0;

    private void Awake()
    {
        ServerManager.instance.OnPlayerSpawned += OnNewPlayerSpawned;
        ServerManager.instance.OnPlayerDisconnected += OnPlayerDisconnected;
    }

    private void OnPlayerDisconnected(object sender, EventArgs e)
    {
        (sender as Player).OnReadyChanged -= PlayerOnReadyChanged;
    }

    private void OnNewPlayerSpawned(object sender, EventArgs e)
    {
        (sender as Player).OnReadyChanged += PlayerOnReadyChanged;
    }

    private void PlayerOnReadyChanged(object sender, EventArgs e)
    {
        if (!_started)
        {
            playerReadyCount = (sender as Player).IsReady ? playerReadyCount + 1 : playerReadyCount - 1;
            Console.WriteLine("Players ready: " + playerReadyCount);

            if (playerReadyCount >= Server.clients.Count)
            {
                if (!_playersReady)
                {
                    //Start countdown
                    _playersReady = true;
                    StartCoroutine(CountdownMatchStart());
                }
            }
            else
                _playersReady = false;
        }
    }

    //public void StartMatch()
    //{
    //    StartCoroutine(BeginStartMatch());
    //}

    //IEnumerator BeginStartMatch()
    //{
    //    _matchDuration = new TimeSpan(0, 0, 10);


    //    yield return WaitForStart();
    //}

    void EndMatch()
    {
        Console.WriteLine("Match has ended!");

        _match = new Matches()
        {
            Begun = _matchStarted,
            Ended = _matchEnded,
            Difficulty = 100,
            MapName = "map1"
        };

        foreach (var client in Server.clients.Values)
        {
            PlayedMatch pM = new PlayedMatch()
            {
                PlayerId = client.player.Username,
                Score = client.player.Score
            };
            _match.PlayedMatch.Add(pM);
        }

        ServerManager.instance.REST.UploadMatch(_match);

    }

    //Kunne evt. laves som en metode der kaldes når en player skifter "ready" state? - event
    //IEnumerator WaitForStart()
    //{
    //    //Wait for atleast 2 players to be ready
    //    Console.WriteLine("Waiting for players to ready up!");

    //    int playerReadyCount;
    //    while (!_started)
    //    {
    //        playerReadyCount = 0;
    //        foreach (var client in Server.clients.Values)
    //        {
    //            if (client.player != null && client.player.IsReady)
    //                playerReadyCount++;
    //        }
    //        if (playerReadyCount >= 1)
    //        {
    //            if (!_playersReady)
    //            {
    //                //Start countdown
    //                _playersReady = true;
    //                StartCoroutine(CountdownMatchStart());
    //            }
    //        }

    //        yield return null;
    //    }
    //}

    IEnumerator CountdownMatchStart()
    {
        _sW.Reset();
        _sW.Start();

        Console.WriteLine("Countdown to match start has started!");

        //Send start countdown to clients here
        while (_playersReady && !_started)
        {
            //if 5 seconds has elasped
            if (_sW.ElapsedMilliseconds >= 5000 && !_started)
            {
                //Stop countdown and start the match here
                StartCoroutine(CountdownMatchEnd());
                yield break;
            }
            yield return null;
        }
        _sW.Stop();
        //Send stop countdown to clients here
        Console.WriteLine("Countdown to match start has stopped!");
        yield return null;
    }

    IEnumerator CountdownMatchEnd()
    {
        _matchStarted = DateTime.UtcNow;
        _started = true;
        Console.WriteLine("Match has started!");
        while (_matchStarted + _matchDuration > DateTime.UtcNow)
        {
            yield return null;
        }
        _matchEnded = DateTime.UtcNow;
        EndMatch();
    }

    private void OnApplicationQuit()
    {
        ServerManager.instance.OnPlayerSpawned -= OnNewPlayerSpawned;
        ServerManager.instance.OnPlayerDisconnected -= OnPlayerDisconnected;
    }
}
