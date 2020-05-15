using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public enum Spell { None, Fireball, ForcePush }

public class SpellCaster : MonoBehaviour {
    public HandPresence LeftController;
    public HandPresence RightController;

    public SpellController LeftSpellController;
    public SpellController RightSpellController;

    private bool _isPressingRight = false;
    private bool _isPressingLeft = false;
    private bool castingSpell = false;

    public GameObject FireballPrefab;
    private CharacterController _characterController = null;
    private Fireball _castFireball;
    private Spell _currentSpell = Spell.None;

    private readonly float _fireballHandOffset = 0.15f;
    private readonly float _fireballMaxSize = 0.2f;


    private Vector3 _pointBetween {
        get {
            return LeftController.Hand.transform.position + (RightController.Hand.transform.position - LeftController.Hand.transform.position) * .5f;
        }
    }


    void Start () {
        _characterController = GetComponent<CharacterController>();
    }

    void Update () {
        RightSpellController.UpdateHandState(RightController);
        LeftSpellController.UpdateHandState(LeftController);
        //RightSpellController.HandState = CheckHandPowerState(RightController);
        //LeftSpellController.HandState = CheckHandPowerState(LeftController);


        if (_currentSpell == Spell.None && RightSpellController.HandState == HandState.Fire && LeftSpellController.HandState == HandState.Fire) {
            if (RightSpellController.FirePercentageCharged >= 1.0f && LeftSpellController.FirePercentageCharged >= 1.0f) {
                if (RightController.CurrentGesture == HandGesture.Open && LeftController.CurrentGesture == HandGesture.Open) {
                    _currentSpell = Spell.Fireball;
                    castingSpell = true;
                    SpawnFireball();
                }
            }
        }

        if (_currentSpell == Spell.Fireball) {
            if (RightSpellController.InputState != HandState.Fire && LeftSpellController.InputState != HandState.Fire) {
                Vector3 velocity = RightController.Velocity * RightSpellController.FirePercentageCharged + LeftController.Velocity * LeftSpellController.FirePercentageCharged;
                velocity /= RightSpellController.FirePercentageCharged + LeftSpellController.FirePercentageCharged;

                _castFireball.Create(velocity);
                _currentSpell = Spell.None;
            }
            else {
                CastFireball();
            }
        }
    }

    void CastFireball () {
        Vector3 fireballPosition = _pointBetween;

        float intermediateRightValue = RightSpellController.FirePercentageCharged * .5f;
        float intermediateLeftValue = (LeftSpellController.FirePercentageCharged * .5f) * -1;
        float delta = intermediateRightValue + intermediateLeftValue + .5f;


        fireballPosition = Vector3.Lerp(LeftController.transform.position - LeftController.transform.up * (.05f + _castFireball.Size) + -LeftController.transform.right * .07f, RightController.transform.position - RightController.transform.up * (.05f + _castFireball.Size) + RightController.transform.right * .07f, delta);
        _castFireball.FollowTarget(fireballPosition);

        float distance = Vector3.Distance(RightController.Hand.transform.position, LeftController.Hand.transform.position);

        distance = Mathf.Clamp(distance - _fireballHandOffset, 0, _fireballMaxSize);

        if (RightSpellController.HandState == HandState.Fire && LeftSpellController.HandState == HandState.Fire) {
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

    HandState CheckHandPowerState (HandPresence hand) {
        if (hand.SecondaryButtonPressed)
            return HandState.Force;        
        else if (hand.PrimaryButtonPressed)
            return HandState.Fire;
        else
            return HandState.Default;
    }


    private void SpawnFireball () {
        GameObject instans = Instantiate(FireballPrefab);
        instans.transform.position = _pointBetween;
        _castFireball = instans.GetComponent<Fireball>();
        _castFireball.Size = 0.0f;
    }
}
