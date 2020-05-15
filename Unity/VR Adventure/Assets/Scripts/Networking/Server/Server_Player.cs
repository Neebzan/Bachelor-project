using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerPlayer {
    public HandDataPacket LeftHand = new HandDataPacket();
    public HandDataPacket RightHand = new HandDataPacket();
    public Vector3 HeadPos;
    public Quaternion HeadRot;
    public int id;
    public Vector3 PointBetween {
        get {
            return LeftHand.HandPosition + (RightHand.HandPosition - LeftHand.HandPosition) * .5f;
        }
    }
}
