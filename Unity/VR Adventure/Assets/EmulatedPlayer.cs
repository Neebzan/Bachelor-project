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

    private void Test () {

        float posMultiplier = 0.1f;

        PlayerDataPacket TestData = new PlayerDataPacket {
            HeadPosition = new Vector3(Mathf.Sin(Time.time), Mathf.Sin(Time.time), Mathf.Sin(Time.time)) * posMultiplier + new Vector3(0, 1.5f, 0),
            HeadRotation = new Quaternion(Mathf.Sin(Time.time), Mathf.Cos(Time.time), Mathf.Sin(Time.time), Mathf.Cos(Time.time)),
            LeftHandDataPacket = new HandDataPacket() {
                HandPosition = new Vector3(Mathf.Sin(Time.time), Mathf.Sin(Time.time), Mathf.Sin(Time.time)) * posMultiplier + new Vector3(-.3f, 0, 0),
                HandRotation = new Quaternion(Mathf.Sin(Time.time), Mathf.Cos(Time.time), Mathf.Sin(Time.time), Mathf.Cos(Time.time)),
                Grip = Mathf.Sin(Time.time),
                Trigger = Mathf.Cos(Time.time)
            },
            RightHandDataPacket = new HandDataPacket() {
                HandPosition = new Vector3(Mathf.Sin(Time.time), Mathf.Sin(Time.time), Mathf.Sin(Time.time)) * posMultiplier + new Vector3(.3f, 0, 0),
                HandRotation = new Quaternion(Mathf.Sin(Time.time), Mathf.Cos(Time.time), Mathf.Sin(Time.time), Mathf.Cos(Time.time)),
                Grip = Mathf.Sin(Time.time),
                Trigger = Mathf.Cos(Time.time)
            },
        };

        Emulate(TestData);
    }

    private void Update () {
        Test();
    }

    void Emulate (PlayerDataPacket data) {
        // Head simulation
        Head.transform.position = data.HeadPosition;
        Head.transform.rotation = data.HeadRotation;

        // Hand simulation
        EmulateHand(LeftHand, data.LeftHandDataPacket);
        EmulateHand(RightHand, data.RightHandDataPacket);
    }

    void EmulateHand (EmulatedHand hand, HandDataPacket data) {
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
}

[Serializable]
public class HandDataPacket {
    public Vector3 HandPosition { get; set; }
    public Quaternion HandRotation { get; set; }
    public float Trigger { get; set; }
    public float Grip { get; set; }
}
