using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromId, Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        string _username = _packet.ReadString();

        System.Console.WriteLine($"Welcome message received from client: {_username}");
        if(_fromId != _clientId)
        {
            System.Console.WriteLine("ID's didn't match and something has gone terribly wrong!");
        }
    }
}
