using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPacketHandler
{
    public static void WelcomeReceived(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        string _username = _packet.ReadString();

        System.Console.WriteLine($"Welcome message received from client: {_username}");

        //Update corresponding player information
        //Server.clients[_clientId].player.UserName = _username;

        Server.clients[_clientId].SpawnPlayer(_username);

        //if(_fromId != _clientId)
        //{
        //    System.Console.WriteLine("ID's didn't match and something has gone terribly wrong!");
        //}
    }

    public static void PlayerMovement(Packet packet)
    {
        //System.Console.WriteLine("Input received!");
        bool[] inputs = new bool[packet.ReadInt()];
        int _playerId = packet.ReadInt();
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        //System.Console.WriteLine($"Input received for playerid: {_playerId}!");
        Server.clients[_playerId].player.SetInput(inputs);

    }

    public static void UdpTestReceived(Packet packet)
    {
        int _id = packet.ReadInt();
        string _msg = packet.ReadString();

        System.Console.WriteLine($"Udp message received: {_msg} from client: {_id}");
    }
}
