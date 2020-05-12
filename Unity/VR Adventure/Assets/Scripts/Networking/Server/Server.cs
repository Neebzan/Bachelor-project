using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }

    private static int playerIndex = 1;

    public static Dictionary<int, ServerClient> clients = new Dictionary<int, ServerClient>();

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;
        //InitServer();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Console.WriteLine("Server started!");
    }

    private static void UDPReceiveCallback(IAsyncResult ar)
    {
        try
        {
            //No specific IP address or Port until set by EndReceive
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(ar, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if(_data.Length < 4)
            {
                return;
            }

            using(Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if(_clientId == 0)
                {
                    Console.WriteLine("Client ID did not match any existing clients! UDPReceiveCallback");
                    return;
                }

                //Check if this is new/first udp packet from client
                if(clients[_clientId].udp.endPoint == null)
                {
                    clients[_clientId].udp.Connect(_clientEndPoint, PacketHandlers.Server);
                    return;
                }

                //Check if the packet is coming from the correct client
                if(clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    //Handle the incoming data
                    clients[_clientId].udp.HandleData(_packet);
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void DisconnectClient(object sender, int _id)
    {
        ServerClient _client;
        if (clients.TryGetValue(_id, out _client))
        {
            _client.Disconnect();
            clients.Remove(_id);
            Debug.Log($"Client: {_id} - Disconnected successfully!");
        }
    }

    private static void TcpConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        //Accept next client
        tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

        Console.WriteLine("Incoming connection from " + _client.Client.RemoteEndPoint);

        if(clients.Count < MaxPlayers)
        {
            clients.Add(playerIndex, new ServerClient(playerIndex));
            clients[playerIndex].Connect(_client);
            playerIndex++;
            return;
        }

        //for (int i = 1; i <= MaxPlayers; i++)
        //{
        //    if (clients[i].tcp.client == null)
        //    {
        //        //clients[i].tcp.Connect(_client, PacketHandlers.Server);
        //        clients[i].Connect(_client);
        //        return;
        //    }
        //}

        Console.WriteLine("Server is full!");

    }

    //public static void DisconnectClient(string id)
    //{
    //    Client _client;
    //    if(clients.TryGetValue(id, out _client))
    //    {
    //        _client.tcp.Disconnect();
    //        clients.Remove(id);
    //        Debug.Log($"Client: {id} - Disconnected successfully!");
    //    }
    //    else
    //    {
    //        Debug.Log($"Attempted to disconnect client: {id} - but the client wasen't found!");
    //    }
    //}

    //private static void InitServer()
    //{
        //for (int i = 1; i <= MaxPlayers; i++)
        //{
        //    clients.Add(i, new ServerClient(i));
        //}

        //packetHandlers = new Dictionary<int, PacketHandler>()
        //{
        //    {(int)ClientPackets.WelcomeReceived, ServerPacketHandler.WelcomeReceived },
        //    {(int)ClientPackets.UdpTestReceived, ServerPacketHandler.UdpTestReceived },
        //    {(int)ClientPackets.PlayerMovement, ServerPacketHandler.PlayerMovement }

        //};
    //}

    public static void Stop()
    {
        tcpListener.Stop();
    }
}
