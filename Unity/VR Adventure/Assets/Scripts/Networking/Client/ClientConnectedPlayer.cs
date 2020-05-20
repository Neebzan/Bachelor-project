using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectedPlayer : MonoBehaviour
{
    public int ID;
    public string Username;
    public int Ping;
    public int Score;

    public EmulatedPlayer emulatedPlayer;
    public EmulatedPlayerUI EmulatedPlayerUI;

    public long LastPlayerUpdateTick = 0;

    public event EventHandler PlayerTick;

    public void Tick(Vector3 headPosition, Quaternion headRotation, HandDataPacket leftHandData, HandDataPacket rightHandData) {
        emulatedPlayer.EmulateHead(headPosition, headRotation);
        emulatedPlayer.EmulateHand(emulatedPlayer.LeftHand, leftHandData);
        emulatedPlayer.EmulateHand(emulatedPlayer.RightHand, rightHandData);

        PlayerTick?.Invoke(this, new EventArgs());
    }


    public void Initialize(int _id, string _username)
    {
        ID = _id;
        Username = _username;
        EmulatedPlayerUI.Username = _username;
    }
}
