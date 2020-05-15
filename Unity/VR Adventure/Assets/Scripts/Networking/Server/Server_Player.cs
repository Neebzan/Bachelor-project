using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerPlayer {
    public ServerVRHand LeftHand = new ServerVRHand ();
    public ServerVRHand RightHand = new ServerVRHand ();

    public Vector3 PointBetween {
        get {
            return LeftHand.HandDataPacket.HandPosition + (RightHand.HandDataPacket.HandPosition - LeftHand.HandDataPacket.HandPosition) * .5f;
        }
    }

    //public HandDataPacket LeftHand = new HandDataPacket();
    //public HandDataPacket RightHand = new HandDataPacket();
    public Vector3 HeadPos;
    public Quaternion HeadRot;
    public int id;

}

public class ServerVRHand {
    public HandDataPacket HandDataPacket { get; set; } = new HandDataPacket();

    public HandGesture CurrentGesture {
        get {
            return HandDataPacket.Grip > .5f ? HandGesture.Grip : HandDataPacket.Trigger > .5f ? HandGesture.Pinch : HandGesture.Open;
        }
    }

    public Vector3 DesiredFireballPosition {
        get {
            return HandDataPacket.HandPosition - Up * (.05f + CastFireball.Size);
        }
    }
    

    public Vector3 Up {
        get {
            return HandDataPacket.HandRotation * Vector3.up;
        }
    }
    public Vector3 Right {
        get {
            return HandDataPacket.HandRotation * Vector3.right;
        }
    }
    public Vector3 Forward {
        get {
            return HandDataPacket.HandRotation * Vector3.forward;
        }
    }
    private Spell _currentSpell;
    public Spell CurrentSpell {
        get {
            return _currentSpell;
        }
        set {
            _currentSpell = value;
        }
    }

    private Fireball castFireball = null;

    public Fireball CastFireball {
        get { return castFireball; }
        set { castFireball = value; }
    }
}
