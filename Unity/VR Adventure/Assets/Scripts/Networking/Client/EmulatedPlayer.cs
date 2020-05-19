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
    public GameObject HeadToBodyTracking;
    public GameObject Body;

    private PlayerClient PlayerClient;
    public bool Test = false;

    private float _headToBodyOffset = 0.2f;

    public void EmulateHead (Vector3 headPos, Quaternion headRot) {
        Head.transform.position = headPos;
        Head.transform.rotation = headRot;
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

    private void FixedUpdate () {
        float headHeight = Mathf.Clamp(Head.transform.position.y, 1.0f, 2.5f) - _headToBodyOffset;

        Body.transform.position = new Vector3(HeadToBodyTracking.transform.position.x, headHeight - .3f, HeadToBodyTracking.transform.position.z);

        Vector3 newForward = Vector3.ProjectOnPlane(Head.transform.forward, Vector3.up);
        Vector3 newDir = Vector3.Lerp(Body.transform.forward, newForward, .1f);
        Body.transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);  
    }

    private void Update () {
        if (Test) {
            if (PlayerClient == null) {
                PlayerClient = GameObject.Find("XR Rig").GetComponent<PlayerClient>();
            }
            else {
                EmulateHead(PlayerClient.Head.transform.position, PlayerClient.Head.transform.rotation);
                //EmulateHand(PlayerClient.LeftHand, PlayerClient.GetHandData(PlayerClient.LeftHand, PlayerClient.LeftHandSpellController);
            }
        }
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
