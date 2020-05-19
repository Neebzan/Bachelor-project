using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerClient : MonoBehaviour {

    public HandPresence LeftHand;
    public HandPresence RightHand;

    private SpellController _leftHandSpellController;
    private SpellController _rightHandSpellController;

    public GameObject Head;

    [HideInInspector]
    public bool Connected = false;


    bool secondaryReleased = true;



    private void Start () {
        ClientPacketHandler.OnClientConnectedToServer += ClientPacketHandler_OnConnectedToServer;
        //Client.instance.ConnectToServer("Placeholder");
        _leftHandSpellController = LeftHand.GetComponent<SpellController>();
        _rightHandSpellController = RightHand.GetComponent<SpellController>();
    }

    private void ClientPacketHandler_OnConnectedToServer () {
        Connected = true;
    }

    private void FixedUpdate () {
        if (!Connected) {
            return;
        }

        ClientPacketSender.PlayerMovement(
            Head.transform.position,
            Head.transform.rotation,
            GetHandData(LeftHand, _leftHandSpellController),
            GetHandData(RightHand, _rightHandSpellController)
            );

        //ClientPacketSender.HeadData(Head.transform.position, Head.transform.rotation);
        //ClientPacketSender.VRLeftHandData(GetHandData(LeftHand, _leftHandSpellController));
        //ClientPacketSender.VRRightHandData(GetHandData(RightHand, _rightHandSpellController));        
    }

    private void Update()
    {
        if(secondaryReleased && RightHand.SecondaryButtonPressed)
        {
            ClientPacketSender.ShootTest(RightHand.transform.forward);
            secondaryReleased = false;
        }
        else if(!RightHand.SecondaryButtonPressed)
        {
            secondaryReleased = true;
        }

    }

    HandDataPacket GetHandData (HandPresence handPresence, SpellController spellController) {
        HandDataPacket data = new HandDataPacket() {
            HandPosition = handPresence.transform.position,
            HandRotation = handPresence.transform.rotation,
            Trigger = handPresence.TriggerValue,
            Grip = handPresence.GripValue,
            Velocity = handPresence.Velocity,
            HandState = spellController.HandState,
            TargetHandState = spellController.TargetHandState,
            StatePower = spellController.StatePower
        };

        return data;
    }
}
