using System;
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

    public static void ShootTestReceived(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        Vector3 dir = _packet.ReadVector3();
        Server.clients[_clientId].player.SpawnTestProjectile(dir);
    }

    //public static void PlayerMovement(Packet packet)
    //{
    //    //System.Console.WriteLine("Input received!");
    //    bool[] inputs = new bool[packet.ReadInt()];
    //    int _playerId = packet.ReadInt();
    //    for (int i = 0; i < inputs.Length; i++)
    //    {
    //        inputs[i] = packet.ReadBool();
    //    }
    //    //System.Console.WriteLine($"Input received for playerid: {_playerId}!");
    //    Server.clients[_playerId].player.SetInput(inputs);

    //}



    public static void VRHeadData(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 pos = _packet.ReadVector3();
        Quaternion rot = _packet.ReadQuaternion();
        Server.clients[id].player.SetHead(pos, rot);
    }

    public static void TimeSync(Packet _packet)
    {
        int id = _packet.ReadInt();
        int clientTime = _packet.ReadInt();
        //Console.WriteLine("TimeSync Received!");
        ServerPacketSender.TimeSync(id, clientTime, DateTime.UtcNow.Millisecond);
    }

    public static void VRLeftHandData(Packet _packet)
    {
        int id = _packet.ReadInt();
        HandDataPacket leftHand = new HandDataPacket()
        {
            HandPosition = _packet.ReadVector3(),
            HandRotation = _packet.ReadQuaternion(),
            Trigger = _packet.ReadFloat(),
            Grip = _packet.ReadFloat(),
            Velocity = _packet.ReadVector3()
        };
        Server.clients[id].player.SetHand(leftHand, true);
    }

    public static void VRRightHandData(Packet _packet)
    {
        int id = _packet.ReadInt();
        HandDataPacket rightHand = new HandDataPacket()
        {
            HandPosition = _packet.ReadVector3(),
            HandRotation = _packet.ReadQuaternion(),
            Trigger = _packet.ReadFloat(),
            Grip = _packet.ReadFloat(),
            Velocity = _packet.ReadVector3()
        };

        Server.clients[id].player.SetHand(rightHand);
    }

    public static void UdpTestReceived(Packet packet)
    {
        int _id = packet.ReadInt();
        string _msg = packet.ReadString();

        System.Console.WriteLine($"Udp message received: {_msg} from client: {_id}");
    }
}
