using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Spell { None, Fireball, ForcePush, LargeFireball }
public class Player : MonoBehaviour {
    public int id;
    public string UserName;
    public Vector3 position;
    //private Fireball _castFireball;
    private readonly float _fireballHandOffset = 0.15f;
    private readonly float _fireballMaxSize = 0.4f;
    private Fireball _largeFireball;

    private float _fireballMergeProgress = 0.0f;
    private bool _mergingFireballs = false;
    private float _fireballMinSize = 0.05f;

    private float speed = 2.0f;

    ServerPlayer vrPlayer = new ServerPlayer();

    private void Start () {
        speed *= Time.fixedDeltaTime;
    }

    public void SetInput (bool input) {

    }

    public void SetHand (HandDataPacket packet, bool left = false) {
        if (left)
            vrPlayer.LeftHand.HandDataPacket = packet;
        else
            vrPlayer.RightHand.HandDataPacket = packet;
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

    void HandleFireballSpell (ServerVRHand hand) {
        if (hand.CurrentSpell == Spell.None) {
            if (hand.HandDataPacket.HandState == HandState.Fire) {
                if (hand.CurrentGesture == HandGesture.Pinch) {
                    if (hand.HandDataPacket.StatePower >= 1.0f) {
                        CastFireball(hand);
                    }
                }
            }
        }
        if (hand.CurrentSpell == Spell.Fireball) {
            if (hand.HandDataPacket.TargetHandState != HandState.Fire) {
                ReleaseFireball(hand);
            }
            else {
                ControlFireball(hand);
            }
        }
    }

    void ReleaseFireball () {
        float rightFirePower = 0.0f;
        float leftFirePower = 0.0f;

        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire) {
            rightFirePower = vrPlayer.RightHand.HandDataPacket.StatePower;
        }
        if (vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire) {
            leftFirePower = vrPlayer.LeftHand.HandDataPacket.StatePower;
        }

        Vector3 velocity = vrPlayer.RightHand.HandDataPacket.Velocity * rightFirePower + vrPlayer.LeftHand.HandDataPacket.Velocity * leftFirePower;
        velocity /= rightFirePower + leftFirePower;

        _largeFireball.Create(velocity);
        vrPlayer.LeftHand.CurrentSpell = Spell.None;
        vrPlayer.RightHand.CurrentSpell = Spell.None;
    }

    void ReleaseFireball (ServerVRHand hand) {
        if (hand.Fireball.Size <= _fireballMinSize) {
            hand.Fireball.Despawn();
        }
        else {
            float rightFirePower = hand.HandDataPacket.StatePower;

            Vector3 velocity = hand.HandDataPacket.Velocity * rightFirePower;
            velocity /= rightFirePower;

            hand.Fireball.Create(velocity);
        }
        hand.CurrentSpell = Spell.None;
    }

    void HandleSingleFireballSpell () {
        // If both hands let go
        if (vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire && vrPlayer.LeftHand.HandDataPacket.TargetHandState != HandState.Fire) {
            ReleaseFireball();
        }
        // If one hand lets go
        else if (vrPlayer.RightHand.HandDataPacket.TargetHandState != vrPlayer.LeftHand.HandDataPacket.TargetHandState) {
            if (vrPlayer.RightHand.HandDataPacket.TargetHandState == HandState.Fire) {
                vrPlayer.RightHand.CurrentSpell = Spell.Fireball;
                vrPlayer.LeftHand.CurrentSpell = Spell.None;

                vrPlayer.RightHand.Fireball = _largeFireball;
            }
            else if (vrPlayer.LeftHand.HandDataPacket.TargetHandState == HandState.Fire) {
                vrPlayer.LeftHand.CurrentSpell = Spell.Fireball;
                vrPlayer.RightHand.CurrentSpell = Spell.None;

                vrPlayer.LeftHand.Fireball = _largeFireball;
            }
        }
        else {
            ControlSingleFireball();
        }

    }

    void CastFireball (ServerVRHand hand) {
        hand.Fireball = ServerManager.instance.SpawnFireball();
        hand.Fireball.Size = 0.0f;
        hand.Fireball.transform.position = hand.DesiredFireballPosition;
        hand.CurrentSpell = Spell.Fireball;
    }

    void ControlFireball (ServerVRHand hand) {
        hand.Fireball.FollowTarget(hand.DesiredFireballPosition);
        ControlFireballSize(hand);
    }

    void ControlFireballSize (ServerVRHand hand) {
        float targetSize = _fireballMaxSize * .3f;

        if (hand.HandDataPacket.HandState == HandState.Fire) {
            float newSize = hand.Fireball.Size;
            if (hand.Fireball.Size < targetSize) {
                newSize = hand.Fireball.Size += Mathf.Clamp(Time.deltaTime * 0.1f, 0, targetSize);
            }
            if (hand.Fireball.Size > targetSize) {
                newSize = hand.Fireball.Size -= Mathf.Clamp(Time.deltaTime * 0.3f, 0, targetSize);
            }

            hand.Fireball.Size = newSize;
        }
    }

    void ControlSingleFireball () {
        Vector3 fireballPosition = vrPlayer.PointBetween;

        float intermediateRightValue = 0;
        float intermediateLeftValue = 0;

        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire) {
            intermediateRightValue = vrPlayer.RightHand.HandDataPacket.StatePower * .5f;
        }
        if (vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire) {
            intermediateLeftValue = (vrPlayer.LeftHand.HandDataPacket.StatePower * .5f) * -1;
        }

        float delta = intermediateRightValue + intermediateLeftValue + .5f;


        fireballPosition = Vector3.Lerp(vrPlayer.LeftHand.HandDataPacket.HandPosition, vrPlayer.RightHand.HandDataPacket.HandPosition, delta);
        fireballPosition = vrPlayer.LeftHand.DesiredFireballPosition + (vrPlayer.RightHand.DesiredFireballPosition - vrPlayer.LeftHand.DesiredFireballPosition) * .5f;
        _largeFireball.FollowTarget(fireballPosition);

        float distance = Vector3.Distance(vrPlayer.RightHand.HandDataPacket.HandPosition, vrPlayer.LeftHand.HandDataPacket.HandPosition);

        //distance = Mathf.Clamp(distance - _fireballHandOffset, 0, _fireballMaxSize);

        distance = Mathf.Clamp(0, distance, _fireballMaxSize);

        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire && vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire) {
            float newSize = _largeFireball.Size;
            if (_largeFireball.Size < distance) {
                newSize = _largeFireball.Size += Mathf.Clamp(Time.deltaTime * 0.1f, 0, distance);
            }
            if (_largeFireball.Size > distance) {
                newSize = _largeFireball.Size -= Mathf.Clamp(Time.deltaTime * 0.3f, 0, distance);
            }

            _largeFireball.Size = newSize;
        }
    }

    public void FireballUpdate () {
        //If controlling a large fireball
        if (vrPlayer.LeftHand.CurrentSpell == vrPlayer.RightHand.CurrentSpell && vrPlayer.RightHand.CurrentSpell == Spell.LargeFireball) {
            HandleSingleFireballSpell();
        }

        // Else if currently not merging fireballs
        else if (!_mergingFireballs && vrPlayer.LeftHand.CurrentSpell != Spell.LargeFireball && vrPlayer.RightHand.CurrentSpell != Spell.LargeFireball) {
            // If hands are close
            if (Vector3.Distance(vrPlayer.LeftHand.HandDataPacket.HandPosition, vrPlayer.RightHand.HandDataPacket.HandPosition) < .5f) {
                // If one or both hands currently have a fireball
                if (vrPlayer.RightHand.CurrentSpell == Spell.Fireball || vrPlayer.LeftHand.CurrentSpell == Spell.Fireball) {
                    // If right only
                    if (vrPlayer.RightHand.CurrentSpell == Spell.Fireball && vrPlayer.LeftHand.CurrentSpell == Spell.None) {
                        if (vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire) {
                            // Set both hands to now control a large fireball
                            vrPlayer.RightHand.CurrentSpell = Spell.LargeFireball;
                            vrPlayer.LeftHand.CurrentSpell = Spell.LargeFireball;

                            // Set the large fireball
                            _largeFireball = vrPlayer.RightHand.Fireball;
                            vrPlayer.LeftHand.Fireball = _largeFireball;
                        }
                    }
                    // If left only
                    if (vrPlayer.LeftHand.CurrentSpell == Spell.Fireball && vrPlayer.RightHand.CurrentSpell == Spell.None) {
                        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire) {
                            // Set both hands to now control a large fireball
                            vrPlayer.RightHand.CurrentSpell = Spell.LargeFireball;
                            vrPlayer.LeftHand.CurrentSpell = Spell.LargeFireball;

                            // Set the large fireball
                            _largeFireball = vrPlayer.LeftHand.Fireball;
                            vrPlayer.RightHand.Fireball = _largeFireball;
                        }
                    }
                    // If both
                    else if (vrPlayer.LeftHand.CurrentSpell == Spell.Fireball && vrPlayer.RightHand.CurrentSpell == Spell.Fireball) {
                        _mergingFireballs = true;
                        _fireballMergeProgress = 0.0f;
                    }
                }
            }
        }

        if (_mergingFireballs) {
            // If fireballs are not at the same position, move them closer
            if (_fireballMergeProgress < 1.0f) {
                vrPlayer.LeftHand.Fireball.FollowTarget(Vector3.Lerp(vrPlayer.LeftHand.DesiredFireballPosition, vrPlayer.PointBetween, _fireballMergeProgress));
                vrPlayer.RightHand.Fireball.FollowTarget(Vector3.Lerp(vrPlayer.RightHand.DesiredFireballPosition, vrPlayer.PointBetween, _fireballMergeProgress));
                _fireballMergeProgress += Time.fixedDeltaTime;
                ControlFireballSize(vrPlayer.LeftHand);
                ControlFireballSize(vrPlayer.RightHand);
            }
            // else If they are close enough
            else {
                // Get the largest of the two
                _largeFireball = vrPlayer.LeftHand.Fireball.Size > vrPlayer.RightHand.Fireball.Size ? vrPlayer.LeftHand.Fireball : vrPlayer.RightHand.Fireball;

                // Despawn the smallest
                if (vrPlayer.RightHand.Fireball == _largeFireball)
                    vrPlayer.LeftHand.Fireball.Despawn();
                else
                    vrPlayer.RightHand.Fireball.Despawn();

                vrPlayer.LeftHand.CurrentSpell = Spell.LargeFireball;
                vrPlayer.RightHand.CurrentSpell = Spell.LargeFireball;

                // We are no longer merging
                _mergingFireballs = false;
            }
        }

        if (!_mergingFireballs && vrPlayer.LeftHand.CurrentSpell != Spell.LargeFireball && vrPlayer.RightHand.CurrentSpell != Spell.LargeFireball) {
            // Manage fireballs as normal
            HandleFireballSpell(vrPlayer.LeftHand);
            HandleFireballSpell(vrPlayer.RightHand);
        }
    }

    public void FixedUpdate () {
        FireballUpdate();

        //Send player information
        ServerPacketSender.HeadData(vrPlayer);
        ServerPacketSender.VRRightHandData(vrPlayer);
        ServerPacketSender.VRLeftHandData(vrPlayer);
    }

    public void SpawnTestProjectile (Vector3 dir) {
        ServerManager.instance.SpawnProjectile(vrPlayer.RightHand.HandDataPacket.HandPosition).Init(dir);
    }
}
