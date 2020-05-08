using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void WelcomeMessage(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Welcome message from server: {msg}");

        Client.instance.id = id;

        //Send a response to server here
    }
}
