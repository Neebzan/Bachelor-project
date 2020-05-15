using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Spell { None, Fireball, ForcePush }
public class Player : MonoBehaviour {
    public int id;
    public string UserName;
    public Vector3 position;
    private Spell _currentSpell = Spell.None;
    private Fireball _castFireball;
    private readonly float _fireballHandOffset = 0.15f;
    private readonly float _fireballMaxSize = 0.2f;

    private float speed = 2.0f;

    ServerPlayer vrPlayer = new ServerPlayer();

    private void Start () {
        speed *= Time.fixedDeltaTime;
    }

    public void SetInput (bool input) {

    }

    public void SetHand (HandDataPacket packet, bool left = false) {
        if (left)
            vrPlayer.LeftHand = packet;
        else
            vrPlayer.RightHand = packet;
    }

    public void SetHead (Vector3 pos, Quaternion rot) {
        vrPlayer.HeadPos = pos;
        vrPlayer.HeadRot = rot;
    }


    internal void Initialize (int id, string userName) {
        UserName = userName;
        this.id = id;
        vrPlayer.id = id;
    }

    void CheckSpellInput () {
        if (_currentSpell == Spell.None) {
            if (vrPlayer.LeftHand.HandState == HandState.Fire && vrPlayer.RightHand.HandState == HandState.Fire) {
                if (vrPlayer.LeftHand.CurrentGesture == HandGesture.Pinch && vrPlayer.RightHand.CurrentGesture == HandGesture.Pinch) {
                    if (vrPlayer.LeftHand.StatePower >= 1.0f && vrPlayer.RightHand.StatePower >= 1.0f) {
                        CastFireball();
                    }
                }
            }
        }
        if (_currentSpell == Spell.Fireball) {
            if (vrPlayer.RightHand.TargetHandState != HandState.Fire && vrPlayer.LeftHand.TargetHandState != HandState.Fire) {
                float rightFirePower = 0.0f;
                float leftFirePower = 0.0f;

                if (vrPlayer.RightHand.HandState == HandState.Fire) {
                    rightFirePower = vrPlayer.RightHand.StatePower;
                }
                if (vrPlayer.LeftHand.HandState == HandState.Fire) {
                    leftFirePower = vrPlayer.LeftHand.StatePower;
                }

                Vector3 velocity = vrPlayer.RightHand.Velocity * rightFirePower + vrPlayer.LeftHand.Velocity * leftFirePower;
                velocity /= rightFirePower + leftFirePower;

                _castFireball.Create(velocity);
                _currentSpell = Spell.None;
            }
            else {
                ControlFireball();
            }
        }
    }

    void CastFireball () {
        _castFireball = ServerManager.instance.SpawnFireball();
        _castFireball.transform.position = vrPlayer.PointBetween;
        _castFireball.Size = 0.0f;
        _currentSpell = Spell.Fireball;
    }

    void ControlFireball () {
        Vector3 fireballPosition = vrPlayer.PointBetween;

        float intermediateRightValue = 0;
        float intermediateLeftValue = 0;

        if (vrPlayer.RightHand.HandState == HandState.Fire) {
            intermediateRightValue = vrPlayer.RightHand.StatePower * .5f;
        }
        if (vrPlayer.LeftHand.HandState == HandState.Fire) {
            intermediateLeftValue = (vrPlayer.LeftHand.StatePower * .5f) * -1;
        }

        float delta = intermediateRightValue + intermediateLeftValue + .5f;


        fireballPosition = Vector3.Lerp(vrPlayer.LeftHand.HandPosition - vrPlayer.LeftHand.Up * (.05f + _castFireball.Size) + -vrPlayer.LeftHand.Right * .07f, vrPlayer.RightHand.HandPosition - vrPlayer.RightHand.Up * (.05f + _castFireball.Size) + vrPlayer.RightHand.Right * .07f, delta);
        _castFireball.FollowTarget(fireballPosition);

        float distance = Vector3.Distance(vrPlayer.RightHand.HandPosition, vrPlayer.LeftHand.HandPosition);

        distance = Mathf.Clamp(distance - _fireballHandOffset, 0, _fireballMaxSize);

        if (vrPlayer.RightHand.HandState == HandState.Fire && vrPlayer.LeftHand.HandState == HandState.Fire) {
            float newSize = _castFireball.Size;
            if (_castFireball.Size < distance) {
                newSize = _castFireball.Size += Mathf.Clamp(Time.deltaTime * 0.1f, 0, distance);
            }
            if (_castFireball.Size > distance) {
                newSize = distance;
            }

            _castFireball.Size = newSize;

        }
    }

    public void FixedUpdate () {
        // Handle fireball manipulation
        CheckSpellInput();

        //Send player information
        ServerPacketSender.HeadData(vrPlayer);
        ServerPacketSender.VRRightHandData(vrPlayer);
        ServerPacketSender.VRLeftHandData(vrPlayer);
    }

    public void SpawnTestProjectile (Vector3 dir) {
        ServerManager.instance.SpawnProjectile(vrPlayer.RightHand.HandPosition).Init(dir);
    }


}
