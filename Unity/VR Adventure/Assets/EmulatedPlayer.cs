using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


// Notes
//
//
// Should hand position be relative to head position?

public class EmulatedPlayer : MonoBehaviour {
    public EmulatedHand LeftHand;
    public EmulatedHand RightHand;
    public GameObject Head;

    
    void Emulate (PlayerDataPacket data) {
        // Head simulation
        Head.transform.position = data.HeadPosition;
        Head.transform.rotation = data.HeadRotation;

        // Hand simulation
        EmulateHand(LeftHand, data.LeftHandDataPacket);
        EmulateHand(RightHand, data.RightHandDataPacket);
    }

    public void EmulateHand (EmulatedHand hand, HandDataPacket data) {
        hand.transform.position = data.HandPosition;
        hand.transform.rotation = data.HandRotation;
        hand.Trigger = data.Trigger;
        hand.Grip = data.Grip;
    }
}

[Serializable]
public class PlayerDataPacket {
    public Vector3 HeadPosition { get; set; }
    public Quaternion HeadRotation { get; set; }
    public HandDataPacket LeftHandDataPacket { get; set; }
    public HandDataPacket RightHandDataPacket { get; set; }
    public bool PrimaryButtonPressed { get; set; }
    public bool SecondaryButtonPressed { get; set; }
}

[Serializable]
public class HandDataPacket {
    public Vector3 HandPosition { get; set; }
    public Quaternion HandRotation { get; set; }
    public float Trigger { get; set; }
    public float Grip { get; set; }
    public Vector3 Velocity { get; set; }
}
