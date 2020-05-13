using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PacketHandler(Packet _packet);

public enum ServerPackets
{
    Welcome,
    UpdTestSent,
    UdpTestReceived,
    SpawnPlayer,
    PlayerPosition,
    PlayerRotation,
    VRRightHandData,
    VRLeftHandData,
    VRHeadData,
    PlayerDisconnected
}

public enum ClientPackets
{
    WelcomeReceived,
    PlayerMovement,
    VRRightHandData,
    VRLeftHandData,
    VRHeadData,
    UdpTestSent,
    UdpTestReceived
}

public static class PacketHandlers
{
    public static Dictionary<int, PacketHandler> Server = new Dictionary<int, PacketHandler>()
        {
            {(int)ClientPackets.WelcomeReceived, ServerPacketHandler.WelcomeReceived },
            {(int)ClientPackets.UdpTestReceived, ServerPacketHandler.UdpTestReceived },
            {(int)ClientPackets.PlayerMovement, ServerPacketHandler.PlayerMovement },
            {(int)ClientPackets.VRRightHandData, ServerPacketHandler.VRRightHandData },
            {(int)ClientPackets.VRLeftHandData, ServerPacketHandler.VRLeftHandData },
            {(int)ClientPackets.VRHeadData, ServerPacketHandler.VRHeadData }

        };

    public static Dictionary<int, PacketHandler> Client = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.Welcome, ClientPacketHandler.WelcomeMessage },
            {(int)ServerPackets.UpdTestSent, ClientPacketHandler.UdpReceiveMessageTest },
            {(int)ServerPackets.SpawnPlayer, ClientPacketHandler.SpawnPlayer },
            {(int)ServerPackets.PlayerPosition, ClientPacketHandler.PlayerPosition },
            {(int)ServerPackets.PlayerDisconnected, ClientPacketHandler.PlayerDisconnected },
            {(int)ServerPackets.VRRightHandData, ClientPacketHandler.VRRightHandData },
            {(int)ServerPackets.VRLeftHandData, ClientPacketHandler.VRLeftHandData },
            {(int)ServerPackets.VRHeadData, ClientPacketHandler.VRHeadData },


        };
}