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

    public static void PlayerMovement(Packet _packet)
    {
        long packetTick = _packet.ReadLong();
        int id = _packet.ReadInt();

        if (Server.clients[id].player.LastUpdateTick < packetTick)
        {
            Server.clients[id].player.LastUpdateTick = packetTick;
            //Head
            Vector3 pos = _packet.ReadVector3();
            Quaternion rot = _packet.ReadQuaternion();

            //Left hand
            HandDataPacket leftHand = new HandDataPacket()
            {
                HandPosition = _packet.ReadVector3(),
                HandRotation = _packet.ReadQuaternion(),
                Trigger = _packet.ReadFloat(),
                Grip = _packet.ReadFloat(),
                Velocity = _packet.ReadVector3(),
                HandState = (HandState)_packet.ReadInt(),
                TargetHandState = (HandState)_packet.ReadInt(),
                StatePower = _packet.ReadFloat(),
            };

            //Right hand
            HandDataPacket rightHand = new HandDataPacket()
            {
                HandPosition = _packet.ReadVector3(),
                HandRotation = _packet.ReadQuaternion(),
                Trigger = _packet.ReadFloat(),
                Grip = _packet.ReadFloat(),
                Velocity = _packet.ReadVector3(),
                HandState = (HandState)_packet.ReadInt(),
                TargetHandState = (HandState)_packet.ReadInt(),
                StatePower = _packet.ReadFloat(),
            };


            Server.clients[id].player.SetHead(pos, rot);
            Server.clients[id].player.SetHand(leftHand, true);
            Server.clients[id].player.SetHand(rightHand);
        }
    }



    //public static void VRHeadData(Packet _packet)
    //{
    //    int packetTick = _packet.ReadInt();
    //    int id = _packet.ReadInt();

    //    Vector3 pos = _packet.ReadVector3();
    //    Quaternion rot = _packet.ReadQuaternion();
    //    Server.clients[id].player.SetHead(pos, rot);
    //}

    public static void TimeSync(Packet _packet)
    {
        int clientTime = _packet.ReadInt();
        int id = _packet.ReadInt();
        //Console.WriteLine("TimeSync Received!");
        ServerPacketSender.TimeSync(id, clientTime, DateTime.UtcNow.Millisecond);
    }

    //public static void VRLeftHandData(Packet _packet)
    //{
    //    int packetTick = _packet.ReadInt();
    //    int id = _packet.ReadInt();

    //    HandDataPacket leftHand = new HandDataPacket()
    //    {
    //        HandPosition = _packet.ReadVector3(),
    //        HandRotation = _packet.ReadQuaternion(),
    //        Trigger = _packet.ReadFloat(),
    //        Grip = _packet.ReadFloat(),
    //        Velocity = _packet.ReadVector3(),
    //        HandState = (HandState)_packet.ReadInt(),
    //        TargetHandState = (HandState)_packet.ReadInt(),
    //        StatePower = _packet.ReadFloat(),
    //    };
    //    Server.clients[id].player.SetHand(leftHand, true);

    //}

    //public static void VRRightHandData(Packet _packet)
    //{
    //    int packetTick = _packet.ReadInt();
    //    int id = _packet.ReadInt();

    //    HandDataPacket rightHand = new HandDataPacket()
    //    {
    //        HandPosition = _packet.ReadVector3(),
    //        HandRotation = _packet.ReadQuaternion(),
    //        Trigger = _packet.ReadFloat(),
    //        Grip = _packet.ReadFloat(),
    //        Velocity = _packet.ReadVector3(),
    //        HandState = (HandState)_packet.ReadInt(),
    //        TargetHandState = (HandState)_packet.ReadInt(),
    //        StatePower = _packet.ReadFloat(),
    //    };

    //    Server.clients[id].player.SetHand(rightHand);

    //}

    public static void UdpTestReceived(Packet packet)
    {
        long packetTick = packet.ReadLong();
        int _id = packet.ReadInt();
        string _msg = packet.ReadString();

        System.Console.WriteLine($"Udp message received: {_msg} from client: {_id}");
    }
}
