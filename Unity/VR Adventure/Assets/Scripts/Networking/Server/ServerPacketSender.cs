using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerPacketSender
{
    public static void WelcomeMessage(int client, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.Welcome))
        {
            packet.Write(msg);
            packet.Write(client);

            SendTCPPacket(client, packet);
            Console.WriteLine("Welcome packet send");
        }
    }

    public static void PlayerPositions()
    {

        foreach (var client in Server.clients.Values)
        {
            if (client.player != null)
            {
                using (Packet _packet = new Packet((int)ServerPackets.PlayerPosition))
                {
                    _packet.Write(client.player.vrPlayer.id);

                    //Head
                    _packet.Write(client.player.vrPlayer.HeadPos);
                    _packet.Write(client.player.vrPlayer.HeadRot);

                    //Left hand
                    _packet.Write(client.player.vrPlayer.LeftHand.HandDataPacket.HandPosition);
                    _packet.Write(client.player.vrPlayer.LeftHand.HandDataPacket.HandRotation);
                    _packet.Write(client.player.vrPlayer.LeftHand.HandDataPacket.Trigger);
                    _packet.Write(client.player.vrPlayer.LeftHand.HandDataPacket.Grip);
                    _packet.Write((int)client.player.vrPlayer.LeftHand.HandDataPacket.HandState);
                    _packet.Write(client.player.vrPlayer.LeftHand.HandDataPacket.StatePower);

                    //Right hand
                    _packet.Write(client.player.vrPlayer.RightHand.HandDataPacket.HandPosition);
                    _packet.Write(client.player.vrPlayer.RightHand.HandDataPacket.HandRotation);
                    _packet.Write(client.player.vrPlayer.RightHand.HandDataPacket.Trigger);
                    _packet.Write(client.player.vrPlayer.RightHand.HandDataPacket.Grip);
                    _packet.Write((int)client.player.vrPlayer.RightHand.HandDataPacket.HandState);
                    _packet.Write(client.player.vrPlayer.RightHand.HandDataPacket.StatePower);

                    SendUDPPacketAll(_packet, client.player.vrPlayer.id);
                }
            }
        }

    }

    public static void SpawnFireball(Fireball fireball)
    {
        using (Packet _packet = new Packet((int)ServerPackets.SpawnFireball))
        {
            _packet.Write(fireball.ID);
            _packet.Write(fireball.transform.position);
            _packet.Write(fireball.Size);

            SendTCPPacketAll(_packet);
        }
    }

    internal static void PlayerReadyStateUpdated(int clientID, bool readyState)
    {
        using (Packet _packet = new Packet((int)ServerPackets.PlayerReadyStateUpdated))
        {
            _packet.Write(clientID);
            _packet.Write(readyState);

            SendTCPPacketAll(_packet, clientID);
        }
    }

    public static void SpawnFireball(int targetClient, Fireball fireball)
    {
        using (Packet _packet = new Packet((int)ServerPackets.SpawnFireball))
        {
            _packet.Write(fireball.ID);
            _packet.Write(fireball.transform.position);
            _packet.Write(fireball.Size);

            SendTCPPacket(targetClient, _packet);
        }
    }

    //public static void UpdateFireball(Fireball fireball)
    //{
    //    using (Packet _packet = new Packet((int)ServerPackets.UpdateFireball))
    //    {
    //        _packet.Write(fireball.ID);
    //        _packet.Write(fireball.transform.position);
    //        _packet.Write(fireball.Size);

    //        SendUDPPacketAll(_packet);
    //    }
    //}

    public static void UpdateFireballsCollection()
    {

        //+4 bytes from enum
        //16 bytes pr fireball
        //Max 30 fireballs pr packet
        int count = 0;

        Packet _packet = new Packet((int)ServerPackets.UpdateFireballs);
        if (ServerManager.instance.Fireballs.Count >= 30)
            _packet.Write(30);
        else
            _packet.Write(ServerManager.instance.Fireballs.Count);

        foreach (var fireball in ServerManager.instance.Fireballs.Values)
        {
            _packet.Write(fireball.ID);
            _packet.Write(fireball.transform.position);
            _packet.Write(fireball.Size);
            count++;
            if (count % 30 == 0)
            {
                SendUDPPacketAll(_packet);
                _packet = new Packet((int)ServerPackets.UpdateFireballs);
                if (ServerManager.instance.Fireballs.Count - count >= 30)
                    _packet.Write(30);
                else
                    _packet.Write(ServerManager.instance.Fireballs.Count - count);
            }
        }
        SendUDPPacketAll(_packet);

    }


    public static void PlayerScoreUpdated(int playerID, int newScore)
    {
        using (Packet _packet = new Packet((int)ServerPackets.PlayerScoreUpdated))
        {
            _packet.Write(playerID);
            _packet.Write(newScore);

            SendTCPPacketAll(_packet);
        }
    }

    internal static void DespawnFireball(int id, bool explode)
    {
        using (Packet _packet = new Packet((int)ServerPackets.DespawnFireball))
        {
            _packet.Write(id);
            _packet.Write(explode);
            ServerManager.instance.Fireballs.Remove(id);

            SendTCPPacketAll(_packet);
        }
    }


    public static void SpawnProjectile(Projectile projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.SpawnProjectile))
        {
            _packet.Write(projectile.id);
            _packet.Write(projectile.transform.position);

            SendTCPPacketAll(_packet);
        }
    }
    public static void DespawnProjectile(Projectile projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.DespawnProjectile))
        {
            _packet.Write(projectile.id);

            SendTCPPacketAll(_packet);
        }
    }

    public static void ProjectilePosition(Projectile projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.ProjectilePosition))
        {
            _packet.Write(projectile.id);
            _packet.Write(projectile.transform.position);
            _packet.Write(projectile.transform.rotation);

            SendUDPPacketAll(_packet);
        }
    }


    public static void SpawnPlayer(int targetClient, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.SpawnPlayer))
        {
            packet.Write(player.ID);
            packet.Write(player.Username);
            packet.Write(player.position);

            SendTCPPacket(targetClient, packet);
        }
    }
    public static void PlayerDisonnected(int id)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerDisconnected))
        {
            packet.Write(id);

            SendTCPPacketAll(packet, id);
        }
    }

    public static void TimeSync(int id, long clientTimeStamp, long serverTime)
    {
        using (Packet _packet = new Packet((int)ServerPackets.TimeSync))
        {
            _packet.Write(clientTimeStamp);
            _packet.Write(serverTime);

            lock (Server.clients)
            {
                _packet.Write(Server.clients.Count - 1); //Write how many client are currently connected, exluding self
                foreach (var client in Server.clients.Values)
                {
                    if (client.id != id)
                    {
                        _packet.Write(client.id);
                        _packet.Write(client.Latency);
                    }
                }
            }
            SendTCPPacket(id, _packet);
        }
    }

    private static void SendTCPPacket(int _clientId, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_clientId].tcp.SendData(_packet);
    }

    private static void SendTCPPacketAll(Packet _packet, int excludeId = -1)
    {
        _packet.WriteLength();
        foreach (var client in Server.clients.Values)
        {
            if (client.id != excludeId)
                client.tcp.SendData(_packet);
        }
    }

    private static void SendUDPPacket(int _clientId, Packet _packet)
    {
        _packet.WriteLengthAndTimeStamp();
        Server.SendUDPData(Server.clients[_clientId].udp.endPoint, _packet);
    }

    private static void SendUDPPacketAll(Packet _packet, int excludeId = -1)
    {
        _packet.WriteLengthAndTimeStamp();
        foreach (var client in Server.clients.Values)
        {
            if (client.id != excludeId)
                Server.SendUDPData(client.udp.endPoint, _packet);
        }

    }
}


//public static void VRRightHandData(ServerPlayer vrPlayer)
//{

//    using (Packet _packet = new Packet((int)ServerPackets.VRRightHandData))
//    {
//        _packet.Write(vrPlayer.id);
//        _packet.Write(vrPlayer.RightHand.HandDataPacket.HandPosition);
//        _packet.Write(vrPlayer.RightHand.HandDataPacket.HandRotation);
//        _packet.Write(vrPlayer.RightHand.HandDataPacket.Trigger);
//        _packet.Write(vrPlayer.RightHand.HandDataPacket.Grip);
//        _packet.Write(vrPlayer.RightHand.HandDataPacket.Velocity);
//        _packet.Write((int)vrPlayer.RightHand.HandDataPacket.HandState);
//        _packet.Write(vrPlayer.RightHand.HandDataPacket.StatePower);

//        SendUDPPacketAll(_packet, vrPlayer.id);
//    }
//}

//public static void VRLeftHandData(ServerPlayer vrPlayer)
//{
//    using (Packet _packet = new Packet((int)ServerPackets.VRLeftHandData))
//    {
//        _packet.Write(vrPlayer.id);
//        _packet.Write(vrPlayer.LeftHand.HandDataPacket.HandPosition);
//        _packet.Write(vrPlayer.LeftHand.HandDataPacket.HandRotation);
//        _packet.Write(vrPlayer.LeftHand.HandDataPacket.Trigger);
//        _packet.Write(vrPlayer.LeftHand.HandDataPacket.Grip);
//        _packet.Write(vrPlayer.LeftHand.HandDataPacket.Velocity);
//        _packet.Write((int)vrPlayer.LeftHand.HandDataPacket.HandState);
//        _packet.Write(vrPlayer.LeftHand.HandDataPacket.StatePower);

//        SendUDPPacketAll(_packet, vrPlayer.id);
//    }
//}

//public static void HeadData(ServerPlayer vrPlayer)
//{
//    using (Packet _packet = new Packet((int)ServerPackets.VRHeadData))
//    {
//        _packet.Write(vrPlayer.id);
//        _packet.Write(vrPlayer.HeadPos);
//        _packet.Write(vrPlayer.HeadRot);

//        SendUDPPacketAll(_packet, vrPlayer.id);
//    }
//}
