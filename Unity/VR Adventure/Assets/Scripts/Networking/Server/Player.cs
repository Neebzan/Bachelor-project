using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public enum Spell { None, Fireball, ForcePush, LargeFireball }

public class Player : MonoBehaviour
{
    public int ID;
    public string Username;
    public int Score;
    public long LastUpdateTick = 0;

    private bool isReady;

    public bool IsReady
    {
        get { return isReady; }
        set
        {
            isReady = value;
            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler OnReadyChanged;

    public ServerPlayer vrPlayer = new ServerPlayer();
    public Vector3 position;

    public GameObject Head;
    public GameObject HeadToBodyTracking;
    public GameObject Body;

    private Fireball _largeFireball;
    private Vector3 BodyOffset;
    private float _fireballMergeProgress = 0.0f;
    private bool _mergingFireballs = false;
    private float _fireballMergeDistance = 0.35f;
    private float _headToBodyOffset = .2f;

    private PlayerClient PlayerClient;
    public bool Test = true;


    private void Awake()
    {
        BodyOffset = transform.position;
    }


    public void SetHand(HandDataPacket packet, bool left = false)
    {
        if (left)
            vrPlayer.LeftHand.HandDataPacket = packet;
        else
            vrPlayer.RightHand.HandDataPacket = packet;
    }
    public void SetHead(Vector3 pos, Quaternion rot)
    {
        vrPlayer.HeadPos = pos;
        vrPlayer.HeadRot = rot;

        Head.transform.position = pos;
        Head.transform.rotation = rot;

        float headHeight = Mathf.Clamp(pos.y, 1.0f, 2.5f) - _headToBodyOffset;

        Body.transform.position = new Vector3(HeadToBodyTracking.transform.position.x, headHeight - .3f, HeadToBodyTracking.transform.position.z);

        Vector3 headForward = rot * Vector3.forward;
        Vector3 newForward = Vector3.ProjectOnPlane(headForward, Vector3.up);
        Vector3 newDir = Vector3.Lerp(Body.transform.forward, newForward, .1f);
        Body.transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
    }

    internal void Initialize(int id, string userName, int score = 0)
    {
        Username = userName;
        this.ID = id;
        vrPlayer.id = id;
        Score = score;
    }


    void ReleaseFireball(ServerVRHand hand)
    {
        if (hand.Fireball.Size <= Fireball.FireballMinSize)
            hand.Fireball.Despawn(false);
        else
            hand.Fireball.Create();

        hand.CurrentSpell = Spell.None;
    }
    void ReleaseFireball()
    {
        if (_largeFireball.Size <= Fireball.FireballMinSize)
            _largeFireball.Despawn(false);
        else
            _largeFireball.Create();

        vrPlayer.LeftHand.CurrentSpell = Spell.None;
        vrPlayer.RightHand.CurrentSpell = Spell.None;
    }

    void HandleFireballSpell(ServerVRHand hand)
    {
        if (hand.CurrentSpell == Spell.None)
        {
            if (hand.HandDataPacket.HandState == HandState.Fire)
            {
                if (hand.CurrentGesture == HandGesture.Pinch)
                {
                    if (hand.HandDataPacket.StatePower >= 1.0f)
                    {
                        CastFireball(hand);
                    }
                }
            }
        }
        if (hand.CurrentSpell == Spell.Fireball)
        {
            if (hand.HandDataPacket.TargetHandState != HandState.Fire)
            {
                ReleaseFireball(hand);
            }
            else
            {
                ControlFireball(hand);
            }
        }
    }
    void HandleFireballSpell()
    {
        // If both hands let go
        if (vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire && vrPlayer.LeftHand.HandDataPacket.TargetHandState != HandState.Fire)
        {
            ReleaseFireball();
        }

        // If one hand lets go
        else if (vrPlayer.RightHand.HandDataPacket.TargetHandState != vrPlayer.LeftHand.HandDataPacket.TargetHandState)
        {
            if (vrPlayer.RightHand.HandDataPacket.Trigger < 1.0f && vrPlayer.LeftHand.HandDataPacket.Trigger < 1.0f)
            {
                ReleaseFireball();
            }

            else if (vrPlayer.RightHand.HandDataPacket.TargetHandState == HandState.Fire)
            {
                vrPlayer.RightHand.CurrentSpell = Spell.Fireball;
                vrPlayer.LeftHand.CurrentSpell = Spell.None;

                vrPlayer.RightHand.Fireball = _largeFireball;
            }
            else if (vrPlayer.LeftHand.HandDataPacket.TargetHandState == HandState.Fire)
            {
                vrPlayer.LeftHand.CurrentSpell = Spell.Fireball;
                vrPlayer.RightHand.CurrentSpell = Spell.None;

                vrPlayer.LeftHand.Fireball = _largeFireball;
            }
        }
        else
        {
            ControlFireball();
        }

    }

    void ControlFireball(ServerVRHand hand)
    {
        // Set target position
        hand.Fireball.FollowTarget(hand.DesiredFireballPosition);
        // Set target size
        ControlFireballSize(hand);
    }
    void ControlFireball()
    {
        // Set target position
        Vector3 fireballPosition = vrPlayer.LeftHand.DesiredFireballPosition + (vrPlayer.RightHand.DesiredFireballPosition - vrPlayer.LeftHand.DesiredFireballPosition) * .5f;
        _largeFireball.FollowTarget(fireballPosition);
        // Set target size
        ControlFireballSize();
    }

    void ControlFireballSize(ServerVRHand hand)
    {
        float targetSize = Fireball.FireballMaxSize;

        if (hand.HandDataPacket.HandState == HandState.Fire)
        {
            hand.Fireball.SetSize(targetSize);
        }
    }
    void ControlFireballSize()
    {
        float targetSize = Mathf.Clamp(Vector3.Distance(vrPlayer.RightHand.HandDataPacket.HandPosition, vrPlayer.LeftHand.HandDataPacket.HandPosition), 0, Fireball.LargeFireballMaxSize);

        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire && vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire)
            _largeFireball.SetSize(targetSize);
    }

    public void FireballUpdate()
    {
        //If controlling a large fireball
        if (vrPlayer.LeftHand.CurrentSpell == vrPlayer.RightHand.CurrentSpell && vrPlayer.RightHand.CurrentSpell == Spell.LargeFireball)
        {
            HandleFireballSpell();
        }

        // Else if currently not merging fireballs
        else if (!_mergingFireballs && vrPlayer.LeftHand.CurrentSpell != Spell.LargeFireball && vrPlayer.RightHand.CurrentSpell != Spell.LargeFireball)
        {
            // If hands are close
            if (Vector3.Distance(vrPlayer.LeftHand.HandDataPacket.HandPosition, vrPlayer.RightHand.HandDataPacket.HandPosition) < _fireballMergeDistance)
            {
                // If one or both hands currently have a fireball
                if (vrPlayer.RightHand.CurrentSpell == Spell.Fireball || vrPlayer.LeftHand.CurrentSpell == Spell.Fireball)
                {
                    // If right only
                    if (vrPlayer.RightHand.CurrentSpell == Spell.Fireball && vrPlayer.LeftHand.CurrentSpell == Spell.None)
                    {
                        if (vrPlayer.LeftHand.HandDataPacket.HandState == HandState.Fire)
                        {
                            // Set both hands to now control a large fireball
                            vrPlayer.RightHand.CurrentSpell = Spell.LargeFireball;
                            vrPlayer.LeftHand.CurrentSpell = Spell.LargeFireball;

                            // Set the large fireball
                            _largeFireball = vrPlayer.RightHand.Fireball;
                            vrPlayer.LeftHand.Fireball = _largeFireball;
                        }
                    }
                    // If left only
                    if (vrPlayer.LeftHand.CurrentSpell == Spell.Fireball && vrPlayer.RightHand.CurrentSpell == Spell.None)
                    {
                        if (vrPlayer.RightHand.HandDataPacket.HandState == HandState.Fire)
                        {
                            // Set both hands to now control a large fireball
                            vrPlayer.RightHand.CurrentSpell = Spell.LargeFireball;
                            vrPlayer.LeftHand.CurrentSpell = Spell.LargeFireball;

                            // Set the large fireball
                            _largeFireball = vrPlayer.LeftHand.Fireball;
                            vrPlayer.RightHand.Fireball = _largeFireball;
                        }
                    }
                    // If both
                    else if (vrPlayer.LeftHand.CurrentSpell == Spell.Fireball && vrPlayer.RightHand.CurrentSpell == Spell.Fireball)
                    {
                        _mergingFireballs = true;
                        _fireballMergeProgress = 0.0f;
                    }
                }
            }
        }

        if (_mergingFireballs)
        {
            // If merging but fireballs get too far apart, or one hand stops enabling fire, then stop the merge
            if (Vector3.Distance(vrPlayer.LeftHand.HandDataPacket.HandPosition, vrPlayer.RightHand.HandDataPacket.HandPosition) > _fireballMergeDistance)
                _mergingFireballs = false;

            if (vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire && vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire)
            {
                _mergingFireballs = false;

                if (vrPlayer.RightHand.HandDataPacket.TargetHandState != HandState.Fire)
                {
                    ReleaseFireball(vrPlayer.RightHand);
                }
                else
                {
                    ReleaseFireball(vrPlayer.LeftHand);
                }
            }


            else
            {
                // If fireballs are not at the same position, move them closer
                if (_fireballMergeProgress < 1.0f)
                {
                    Vector3 mergePosition = vrPlayer.LeftHand.DesiredFireballPosition + (vrPlayer.RightHand.DesiredFireballPosition - vrPlayer.LeftHand.DesiredFireballPosition) * .5f;
                    vrPlayer.LeftHand.Fireball.FollowTarget(Vector3.Lerp(vrPlayer.LeftHand.DesiredFireballPosition, mergePosition, _fireballMergeProgress));
                    vrPlayer.RightHand.Fireball.FollowTarget(Vector3.Lerp(vrPlayer.RightHand.DesiredFireballPosition, mergePosition, _fireballMergeProgress));
                    _fireballMergeProgress += Time.fixedDeltaTime * 2.0f;
                    ControlFireballSize(vrPlayer.LeftHand);
                    ControlFireballSize(vrPlayer.RightHand);
                }
                // else If they are close enough
                else
                {
                    // Get the largest of the two
                    _largeFireball = vrPlayer.LeftHand.Fireball.Size > vrPlayer.RightHand.Fireball.Size ? vrPlayer.LeftHand.Fireball : vrPlayer.RightHand.Fireball;

                    // Despawn the smallest
                    if (vrPlayer.RightHand.Fireball == _largeFireball)
                    {
                        vrPlayer.LeftHand.Fireball.Despawn(false);
                        vrPlayer.LeftHand.Fireball = _largeFireball;
                    }
                    else
                    {
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

        if (!_mergingFireballs && vrPlayer.LeftHand.CurrentSpell != Spell.LargeFireball && vrPlayer.RightHand.CurrentSpell != Spell.LargeFireball)
        {
            // Manage fireballs as normal
            HandleFireballSpell(vrPlayer.LeftHand);
            HandleFireballSpell(vrPlayer.RightHand);
        }
    }

    void CastFireball(ServerVRHand hand)
    {
        hand.Fireball = ServerManager.instance.SpawnFireball(ID);
        hand.Fireball.Size = 0.0f;
        hand.Fireball.transform.position = hand.DesiredFireballPosition;
        hand.CurrentSpell = Spell.Fireball;
    }

    public void FixedUpdate()
    {
        FireballUpdate();
        if (Test)
        {
            if (PlayerClient == null)
            {
                PlayerClient = GameObject.Find("XR Rig").GetComponent<PlayerClient>();
            }
            else
            {
                SetHead(PlayerClient.Head.transform.position, PlayerClient.Head.transform.rotation);
                //EmulateHand(PlayerClient.LeftHand, PlayerClient.GetHandData(PlayerClient.LeftHand, PlayerClient.LeftHandSpellController);
            }
        }
    }
}
