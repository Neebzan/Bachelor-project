using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrPlayerServer {
    public HandDataPacket LeftHand = new HandDataPacket();
    public HandDataPacket RightHand = new HandDataPacket();
    public Vector3 HeadPos;
    public Quaternion HeadRot;
    public int id;
}
