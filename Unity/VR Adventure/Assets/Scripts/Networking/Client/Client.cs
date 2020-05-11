using DatabaseREST.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 13000;
    public int id;

    public TCP tcp;


    public bool isConnected = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tcp = new TCP();
    }

    public void ConnectToServer()
    {
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient client;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        private Packet incomingPacket;

        public void Connect()
        {
            client = new TcpClient()
            {
                SendBufferSize = dataBufferSize,
                ReceiveBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            client.BeginConnect(instance.ip, instance.port, ConnectCallback, client);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            try
            {
                client.EndConnect(_result);

                Debug.Log("Connecting!");

                if (!client.Connected)
                    return;

                Debug.Log("Connected!");

                stream = client.GetStream();

                //Initialize the packet class
                incomingPacket = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {

                Debug.Log(e);
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int bytesReceived = stream.EndRead(_result);
                //
                if (bytesReceived <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] data = new byte[bytesReceived];
                Array.Copy(receiveBuffer, data, bytesReceived);

                incomingPacket.Reset(HandleData(data));

                //Wait for next data
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            }
            catch (Exception e)
            {

                throw;
            }
        }

        private bool HandleData(byte[] _data)
        {
            int packetLength = 0;

            incomingPacket.SetBytes(_data);

            //Check if there 4 or more bytes in the packet (size of int is 4)
            if(incomingPacket.UnreadLength() >= 4)
            {
                return true;
            }
            return true;
        }

    }
    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.client.Close();
            //udp.socket.Close();

            Debug.Log("Disconnected from server.");
        }
    }
}
