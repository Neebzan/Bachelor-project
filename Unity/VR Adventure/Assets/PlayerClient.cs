using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerClient : MonoBehaviour {

    public HandPresence LeftHand;
    public HandPresence RightHand;
    public GameObject Head;

    [HideInInspector]
    public bool Connected = false;


    private void Start () {
        ClientPacketHandler.OnConnectedToServer += ClientPacketHandler_OnConnectedToServer;
        Client.instance.ConnectToServer("Placeholder");
    }

    private void ClientPacketHandler_OnConnectedToServer () {
        Connected = true;
    }

    private void FixedUpdate () {
        if (!Connected) {
            return;
        }

        ClientPacketSender.HeadData(Head.transform.position, Head.transform.rotation);
        ClientPacketSender.VRLeftHandData(GetHandData(LeftHand));
        ClientPacketSender.VRRightHandData(GetHandData(RightHand));
    }


    HandDataPacket GetHandData (HandPresence hand) {
        HandDataPacket data = new HandDataPacket() {
            HandPosition = hand.transform.position,
            HandRotation = hand.transform.rotation,
            Trigger = hand.TriggerValue,
            Grip = hand.GripValue,
            Velocity = hand.Velocity
        };
        return data;
    }
}
