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
        }
        Console.WriteLine($"Welcome message sent to client {client}");
    }

    public static void UdpTestMessage(int client)
    {
        using (Packet packet = new Packet((int)ServerPackets.UpdTestSent))
        {
            packet.Write("This is a UDP test from the Server!");

            SendUDPPacket(client, packet);
        }
        Console.WriteLine($"UdpTest message sent to client {client}");
    }


    public static void PlayerPostion(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerPosition))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);

            SendUDPPacketAll(packet);
        }
    }

    public static void VRRightHandData(VrPlayerServer vrPlayer)
    {

        using (Packet _packet = new Packet((int)ServerPackets.VRRightHandData))
        {
            _packet.Write(vrPlayer.id);
            _packet.Write(vrPlayer.RightHand.HandPosition);
            _packet.Write(vrPlayer.RightHand.HandRotation);
            _packet.Write(vrPlayer.RightHand.Trigger);
            _packet.Write(vrPlayer.RightHand.Grip);
            _packet.Write(vrPlayer.RightHand.Velocity);

            SendUDPPacketAll(_packet, vrPlayer.id);
        }
    }

    public static void VRLeftHandData(VrPlayerServer vrPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.VRLeftHandData))
        {
            _packet.Write(vrPlayer.id);
            _packet.Write(vrPlayer.LeftHand.HandPosition);
            _packet.Write(vrPlayer.LeftHand.HandRotation);
            _packet.Write(vrPlayer.LeftHand.Trigger);
            _packet.Write(vrPlayer.LeftHand.Grip);
            _packet.Write(vrPlayer.LeftHand.Velocity);

            SendUDPPacketAll(_packet, vrPlayer.id);
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

    public static void HeadData(VrPlayerServer vrPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.VRHeadData))
        {
            _packet.Write(vrPlayer.id);
            _packet.Write(vrPlayer.HeadPos);
            _packet.Write(vrPlayer.HeadRot);

            SendUDPPacketAll(_packet, vrPlayer.id);
        }
    }


    public static void SpawnPlayer(int targetClient, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.SpawnPlayer))
        {
            packet.Write(player.id);
            packet.Write(player.UserName);
            packet.Write(player.position);

            Console.WriteLine("Send SpawnPlayer message to client: " + targetClient);
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
        _packet.WriteLength();
        Server.SendUDPData(Server.clients[_clientId].udp.endPoint, _packet);
    }

    private static void SendUDPPacketAll(Packet _packet, int excludeId = -1)
    {
        _packet.WriteLength();
        foreach (var client in Server.clients.Values)
        {
            if (client.id != excludeId)
                Server.SendUDPData(client.udp.endPoint, _packet);
        }

    }
}

