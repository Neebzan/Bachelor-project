using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PacketHandler(Packet _packet);

public enum HandState {
    Default = 0,
    Fire = 1,
    Force = 2
}
public enum ServerPackets
{
    Welcome,

    UpdTestSent,
    UdpTestReceived,
    SpawnPlayer,

    PlayerPosition,
    PlayerRotation,

    //VRRightHandData,
    //VRLeftHandData,
    //VRHeadData,

    PlayerDisconnected,

    SpawnProjectile,
    ProjectilePosition,
    DespawnProjectile,

    TimeSync,

    SpawnFireball,
    //UpdateFireball,
    UpdateFireballs,
    DespawnFireball
}

public enum ClientPackets
{
    WelcomeReceived,

    PlayerMovement,
    //PlayerInput,

    ShootTest,

    //VRRightHandData,
    //VRLeftHandData,
    //VRHeadData,
    UdpTestSent,
    UdpTestReceived,

    TimeSync
}

public static class PacketHandlers
{
    public static Dictionary<int, PacketHandler> Server = new Dictionary<int, PacketHandler>()
        {
            {(int)ClientPackets.WelcomeReceived, ServerPacketHandler.WelcomeReceived },
            {(int)ClientPackets.UdpTestReceived, ServerPacketHandler.UdpTestReceived },

            //{(int)ClientPackets.ShootTest, ServerPacketHandler.ShootTestReceived },

            {(int)ClientPackets.PlayerMovement, ServerPacketHandler.PlayerMovement },

            //{(int)ClientPackets.VRRightHandData, ServerPacketHandler.VRRightHandData },
            //{(int)ClientPackets.VRLeftHandData, ServerPacketHandler.VRLeftHandData },
            //{(int)ClientPackets.VRHeadData, ServerPacketHandler.VRHeadData },

            {(int)ClientPackets.TimeSync, ServerPacketHandler.TimeSync },

        };

    public static Dictionary<int, PacketHandler> Client = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.Welcome, ClientPacketHandler.WelcomeMessage },

            {(int)ServerPackets.UpdTestSent, ClientPacketHandler.UdpReceiveMessageTest },

            {(int)ServerPackets.SpawnPlayer, ClientPacketHandler.SpawnPlayer },

            {(int)ServerPackets.PlayerPosition, ClientPacketHandler.PlayerPosition },
            {(int)ServerPackets.PlayerDisconnected, ClientPacketHandler.PlayerDisconnected },

            //{(int)ServerPackets.VRRightHandData, ClientPacketHandler.VRRightHandData },
            //{(int)ServerPackets.VRLeftHandData, ClientPacketHandler.VRLeftHandData },
            //{(int)ServerPackets.VRHeadData, ClientPacketHandler.VRHeadData },

            //{(int)ServerPackets.SpawnProjectile, ClientPacketHandler.SpawnProjectile },
            //{(int)ServerPackets.ProjectilePosition, ClientPacketHandler.ProjectilePosition },
            //{(int)ServerPackets.DespawnProjectile, ClientPacketHandler.DespawnProjectile },

            {(int)ServerPackets.SpawnFireball, ClientPacketHandler.SpawnFireball},
            //{(int)ServerPackets.UpdateFireball, ClientPacketHandler.UpdateFireball},
            {(int)ServerPackets.UpdateFireballs, ClientPacketHandler.UpdateFireballs},
            {(int)ServerPackets.DespawnFireball, ClientPacketHandler.DespawnFireball},

            {(int)ServerPackets.TimeSync, ClientPacketHandler.TimeSync },
        };
}