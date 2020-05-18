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

    private Fireball _largeFireball;

    private float _fireballMergeProgress = 0.0f;
    private bool _mergingFireballs = false;
    private float _fireballMergeDistance = 0.35f;

    private float speed = 2.0f;

    public long LastUpdateTick = 0;

    public ServerPlayer vrPlayer = new ServerPlayer();

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
                //if (hand.HandDataPacket.Grip > 0) {
                //    ReleaseFireball(hand, new Vector3(5, 0, 0));
                //}
                //else {
                ReleaseFireball(hand, Vector3.zero);
                //}
            }
            else {
                ControlFireball(hand);
            }
        }
    }

    void ReleaseFireball () {
        if (_largeFireball.Size <= Fireball.FireballMinSize) {
            _largeFireball.Despawn(false);
        }
        else {
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
        }

        vrPlayer.LeftHand.CurrentSpell = Spell.None;
        vrPlayer.RightHand.CurrentSpell = Spell.None;
    }

    void ReleaseFireball (ServerVRHand hand, Vector3 initialVelocity) {
        if (hand.Fireball.Size <= Fireball.FireballMinSize) {
            hand.Fireball.Despawn(false);
        }
        else {
            Vector3 velocity = Vector3.zero;
            if (initialVelocity != Vector3.zero) {
                velocity = initialVelocity;
            }
            else {
                float rightFirePower = hand.HandDataPacket.StatePower;
                velocity = hand.HandDataPacket.Velocity * rightFirePower;
                velocity /= rightFirePower;
            }

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
            if (vrPlayer.RightHand.HandDataPacket.Trigger <1.0f && vrPlayer.LeftHand.HandDataPacket.Trigger < 1.0f) {
                ReleaseFireball();
            }

            else if (vrPlayer.RightHand.HandDataPacket.TargetHandState == HandState.Fire) {
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
        float targetSize = Fireball.FireballMaxSize * .3f;

        if (hand.HandDataPacket.HandState == HandState.Fire) {
            hand.Fireball.SetSize(targetSize);
        }
    }

    void ControlSingleFireball () {      
        Vector3 fireballPosition = vrPlayer.LeftHand.DesiredFireballPosition + (vrPlayer.RightHand.DesiredFireballPosition - vrPlayer.LeftHand.DesiredFireballPosition) * .5f;
        _largeFireball.FollowTarget(fireballPosition);

        float targetSize = Vector3.Distance(vrPlayer.RightHand.HandDataPacket.HandPosition, vrPlayer.LeftHand.HandDataPacket.HandPosition);
        targetSize = Mathf.Clamp(0, targetSize, Fireball.FireballMaxSize);

        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire && vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire) {
            _largeFireball.SetSize(targetSize);
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
            if (Vector3.Distance(vrPlayer.LeftHand.HandDataPacket.HandPosition, vrPlayer.RightHand.HandDataPacket.HandPosition) < _fireballMergeDistance) {
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
            // If merging but fireballs get too far apart, or one hand stops enabling fire, then stop the merge
            if (Vector3.Distance(vrPlayer.LeftHand.HandDataPacket.HandPosition, vrPlayer.RightHand.HandDataPacket.HandPosition) > _fireballMergeDistance)
                _mergingFireballs = false;

            if (vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire && vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire) {
                _mergingFireballs = false;

                if (vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire) {
                    vrPlayer.RightHand.Fireball.Despawn(false);
                }
                else {
                    vrPlayer.LeftHand.Fireball.Despawn(false);
                }
            }


            else {
                // If fireballs are not at the same position, move them closer
                if (_fireballMergeProgress < 1.0f) {
                    Vector3 mergePosition = vrPlayer.LeftHand.DesiredFireballPosition + (vrPlayer.RightHand.DesiredFireballPosition - vrPlayer.LeftHand.DesiredFireballPosition) * .5f;
                    vrPlayer.LeftHand.Fireball.FollowTarget(Vector3.Lerp(vrPlayer.LeftHand.DesiredFireballPosition, mergePosition, _fireballMergeProgress));
                    vrPlayer.RightHand.Fireball.FollowTarget(Vector3.Lerp(vrPlayer.RightHand.DesiredFireballPosition, mergePosition, _fireballMergeProgress));
                    _fireballMergeProgress += Time.fixedDeltaTime * 2.0f;
                    ControlFireballSize(vrPlayer.LeftHand);
                    ControlFireballSize(vrPlayer.RightHand);
                }
                // else If they are close enough
                else {
                    // Get the largest of the two
                    _largeFireball = vrPlayer.LeftHand.Fireball.Size > vrPlayer.RightHand.Fireball.Size ? vrPlayer.LeftHand.Fireball : vrPlayer.RightHand.Fireball;

                    // Despawn the smallest
                    if (vrPlayer.RightHand.Fireball == _largeFireball) {
                        vrPlayer.LeftHand.Fireball.Despawn(false);
                        vrPlayer.LeftHand.Fireball = _largeFireball;
                    }
                    else {
                        vrPlayer.RightHand.Fireball.Despawn(false);
                        vrPlayer.RightHand.Fireball = _largeFireball;
                    }

                    vrPlayer.LeftHand.CurrentSpell = Spell.LargeFireball;
                    vrPlayer.RightHand.CurrentSpell = Spell.LargeFireball;

                    // We are no longer merging
                    _mergingFireballs = false;
                }
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

        //ServerPacketSender.HeadData(vrPlayer);
        //ServerPacketSender.VRRightHandData(vrPlayer);
        //ServerPacketSender.VRLeftHandData(vrPlayer);
    }

    public void SpawnTestProjectile (Vector3 dir) {
        ServerManager.instance.SpawnProjectile(vrPlayer.RightHand.HandDataPacket.HandPosition).Init(dir);
    }
}
