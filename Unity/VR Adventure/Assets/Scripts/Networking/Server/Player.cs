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
                float rightFirePower = 0.0f;

                if (hand.HandDataPacket.HandState == HandState.Fire) {
                    rightFirePower = hand.HandDataPacket.StatePower;
                }

                Vector3 velocity = hand.HandDataPacket.Velocity * rightFirePower;
                velocity /= rightFirePower;

                hand.CastFireball.Create(velocity);
                hand.CurrentSpell = Spell.None;
            }
            else {
                ControlFireball(hand);
            }
        }
    }

    void HandleSingleFireballSpell () {
        if (vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire && vrPlayer.LeftHand.HandDataPacket.TargetHandState != HandState.Fire) {
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
        else {
            ControlSingleFireball();
        }

    }

    void CastFireball (ServerVRHand hand) {
        hand.CastFireball = ServerManager.instance.SpawnFireball();
        hand.CastFireball.Size = 0.0f;
        hand.CastFireball.transform.position = hand.DesiredFireballPosition;
        hand.CurrentSpell = Spell.Fireball;
    }

    void ControlFireball (ServerVRHand hand) {
        hand.CastFireball.FollowTarget(hand.DesiredFireballPosition);

        float targetSize = _fireballMaxSize * .3f;

        if (hand.HandDataPacket.HandState == HandState.Fire) {
            float newSize = hand.CastFireball.Size;
            if (hand.CastFireball.Size < targetSize) {
                newSize = hand.CastFireball.Size += Mathf.Clamp(Time.deltaTime * 0.1f, 0, targetSize);
            }
            if (hand.CastFireball.Size > targetSize) {
                newSize = targetSize;
            }

            hand.CastFireball.Size = newSize;
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


        fireballPosition = Vector3.Lerp(vrPlayer.LeftHand.HandDataPacket.HandPosition - vrPlayer.LeftHand.Up * (.05f + _largeFireball.Size) + -vrPlayer.LeftHand.Right * .07f, vrPlayer.RightHand.HandDataPacket.HandPosition - vrPlayer.RightHand.Up * (.05f + _largeFireball.Size) + vrPlayer.RightHand.Right * .07f, delta);
        _largeFireball.FollowTarget(fireballPosition);

        float distance = Vector3.Distance(vrPlayer.RightHand.HandDataPacket.HandPosition, vrPlayer.LeftHand.HandDataPacket.HandPosition);

        distance = Mathf.Clamp(distance - _fireballHandOffset, 0, _fireballMaxSize);

        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire && vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire) {
            float newSize = _largeFireball.Size;
            if (_largeFireball.Size < distance) {
                newSize = _largeFireball.Size += Mathf.Clamp(Time.deltaTime * 0.1f, 0, distance);
            }
            if (_largeFireball.Size > distance) {
                newSize = distance;
            }

            _largeFireball.Size = newSize;

        }
    }

    public void FixedUpdate () {
        // If hands are close
        if (Vector3.Distance(vrPlayer.LeftHand.HandDataPacket.HandPosition, vrPlayer.RightHand.HandDataPacket.HandPosition) < .5f) {
            if (!_mergingFireballs && vrPlayer.LeftHand.CurrentSpell == Spell.Fireball && vrPlayer.RightHand.CurrentSpell == Spell.Fireball) {
                // Begin merge
                _mergingFireballs = true;
            }
        }
        else {
            // Demerge, and spawn two smaller fireballs
        }


        // If merge has begun
        if (_mergingFireballs) {
            // If fireballs are not at the same position
            // Move closer
            if (_fireballMergeProgress < 1.0f) {
                Console.WriteLine("Player begin merge");
                vrPlayer.LeftHand.CastFireball.FollowTarget(Vector3.Lerp(vrPlayer.LeftHand.DesiredFireballPosition, vrPlayer.PointBetween, _fireballMergeProgress));
                vrPlayer.RightHand.CastFireball.FollowTarget(Vector3.Lerp(vrPlayer.RightHand.DesiredFireballPosition, vrPlayer.PointBetween, _fireballMergeProgress));
                _fireballMergeProgress += Time.fixedDeltaTime;
            }
            // If they are close enough
            // Merge
            else {
                // Fireballs are close, so despawn and spawn a larger one
                float largestSize = 0.0f;
                if (vrPlayer.LeftHand.CastFireball.Size > largestSize) {
                    largestSize = vrPlayer.LeftHand.CastFireball.Size;
                }
                if (vrPlayer.RightHand.CastFireball.Size > largestSize) {
                    largestSize = vrPlayer.RightHand.CastFireball.Size;
                }


                vrPlayer.LeftHand.CastFireball.Despawn();
                vrPlayer.RightHand.CastFireball.Despawn();

                _largeFireball = ServerManager.instance.SpawnFireball();
                _largeFireball.Size = largestSize;
                _largeFireball.transform.position = vrPlayer.PointBetween;

                vrPlayer.LeftHand.CurrentSpell = Spell.LargeFireball;
                vrPlayer.RightHand.CurrentSpell = Spell.LargeFireball;

                // We are no longer merging
                _mergingFireballs = false;
            }
        }

        //If not merging fireballs
        else {
            // If controlling large fireball
            if (vrPlayer.RightHand.CurrentSpell == Spell.LargeFireball) {
                Console.WriteLine("Spawn large fireball");
                HandleSingleFireballSpell();
            }
            // If not
            else {
                Console.WriteLine("Default fireball logic");
                _fireballMergeProgress = 0.0f;
                HandleFireballSpell(vrPlayer.LeftHand);
                HandleFireballSpell(vrPlayer.RightHand);
            }
        }


        //Send player information
        ServerPacketSender.HeadData(vrPlayer);
        ServerPacketSender.VRRightHandData(vrPlayer);
        ServerPacketSender.VRLeftHandData(vrPlayer);
    }

    public void SpawnTestProjectile (Vector3 dir) {
        ServerManager.instance.SpawnProjectile(vrPlayer.RightHand.HandDataPacket.HandPosition).Init(dir);
    }
}
