using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum HandGesture { Open, Pinch, Grip }

public class EmulatedPlayer : MonoBehaviour {
    public EmulatedHand LeftHand;
    public EmulatedHand RightHand;
    public GameObject Head;
    public GameObject Body;

    private float _headToBodyOffset = 0.2f;

    public void EmulateHead (Vector3 headPos, Quaternion headRot) {
        Head.transform.position = headPos;
        Head.transform.rotation = headRot;

        float headHeight = Mathf.Clamp(headPos.y, 1.0f, 2.5f) - _headToBodyOffset;

        Body.transform.localScale = new Vector3(Body.transform.localScale.x, headHeight * .5f, Body.transform.localScale.z);
        Body.transform.position = new Vector3(Head.transform.position.x, headHeight * .5f, Head.transform.position.z);
    }

    public void EmulateHand (EmulatedHand hand, HandDataPacket data) {
        hand.transform.position = data.HandPosition;
        hand.transform.rotation = data.HandRotation;
        hand.Trigger = data.Trigger;
        hand.Grip = data.Grip;
        hand.HandState = data.HandState;
        hand.StatePower = data.StatePower;

        hand.Animate();
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
    public HandState HandState { get; set; }
    public HandState TargetHandState { get; set; }
    public float StatePower { get; set; }
    }
