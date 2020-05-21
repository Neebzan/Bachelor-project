using DatabaseREST.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Match : MonoBehaviour
{
    Matches _match;
    bool _playersReady;
    Stopwatch _sW = new Stopwatch();

    public void StartMatch()
    {
        _match = new Matches()
        {
            Begun = DateTime.UtcNow,
            Difficulty = 100,
            MapName = "map1"
        };
    }

    public void EndMatch()
    {

    }

    //Kunne evt. laves som en metode der kaldes når en player skifter "ready" state? - event
    IEnumerator WaitForStart()
    {
        //Wait for atleast 2 players to be ready
        bool start = false;
        int playerReadyCount;
        while (!start)
        {
            playerReadyCount = 0;
            foreach (var client in Server.clients.Values)
            {
                if (client.player.IsReady)
                    playerReadyCount++;
            }
            if (playerReadyCount >= 2)
            {
                //Start countdown
                yield return CountdownMatchStart();
            }

            yield return null;
        }
    }

    IEnumerator CountdownMatchStart()
    {
        _sW.Start();
        //Send start countdown to clients here
        while(_playersReady)
        {
            //if 5 seconds has elasped
            if(_sW.ElapsedMilliseconds >= 5000)
            {
                //Stop countdown and start the match here
            }
            yield return null;
        }
        //Send stop countdown to clients here
        yield return null;
    }
}
