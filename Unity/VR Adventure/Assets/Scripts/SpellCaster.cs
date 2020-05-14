﻿using System;
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




    private Vector3 _pointBetween {
        get {
            return LeftController.Hand.transform.position + (RightController.Hand.transform.position - LeftController.Hand.transform.position) * .5f;
        }
    }


    void Start () {
        _characterController = GetComponent<CharacterController>();
    }

    void Update () {
        if (!RightSpellController.UsingFire)
            RightSpellController.UsingForce = CheckHandUsingForce(RightController);
        if (!RightSpellController.UsingForce)
            RightSpellController.UsingFire = CheckHandUsingFire(RightController);

        if (!LeftSpellController.UsingFire)
            LeftSpellController.UsingForce = CheckHandUsingForce(LeftController);
        if (!LeftSpellController.UsingForce)
            LeftSpellController.UsingFire = CheckHandUsingFire(LeftController);

        if (_currentSpell == Spell.None && RightSpellController.UsingFire && LeftSpellController.UsingFire) {
            if (RightSpellController.FireCharge >= 1.0f && LeftSpellController.FireCharge >= 1.0f) {
                if (RightController.CurrentGesture == HandGesture.Open && LeftController.CurrentGesture == HandGesture.Open) {
                    _currentSpell = Spell.Fireball;
                    castingSpell = true;
                    SpawnFireball();
                }
            }
        }

        if (_currentSpell == Spell.Fireball) {
            if (!RightSpellController.UsingFire && !LeftSpellController.UsingFire) {
                _castFireball.Create(RightController.Velocity, LeftController.Velocity);
                _currentSpell = Spell.None;
            }
            else {
                CastFireball();
            }
        }
    }

    void CastFireball () {
        Vector3 fireballPosition = _pointBetween;



        //float intermediateRightValue = RightSpellController.FireCharge * .5f;
        //float intermediateLeftValue = (LeftSpellController.FireCharge * .5f) * -1;
        //float delta = intermediateRightValue + intermediateLeftValue + .5f;

        //fireballPosition = Vector3.Lerp(LeftController.transform.position, RightController.transform.position, delta);





        _castFireball.FollowTarget(fireballPosition);

        float distance = Vector3.Distance(RightController.Hand.transform.position, LeftController.Hand.transform.position);
        float offset = 0.15f;
        float maxSize = 0.2f;
        distance = Mathf.Clamp(distance - offset, 0, maxSize);

        if (_castFireball.Size != distance) {
            float newSize = 0.0f;
            if (_castFireball.Size < distance) {
                newSize = _castFireball.Size += Mathf.Clamp(Time.deltaTime * 0.1f, 0, distance);
            }
            if (_castFireball.Size > distance) {
                newSize = distance;
            }

            _castFireball.Size = newSize;
        }

        //if (LeftSpellController.FireCharge == 0.0f) {
        //    if (true) {

        //    }
        //}

        //intermediateRightValue = RightSpellController.FireCharge * .5f;
        //intermediateLeftValue = LeftSpellController.FireCharge * .5f;
        //delta = intermediateRightValue + intermediateLeftValue;

        //_castFireball.Size *= delta;
    }


    bool CheckHandUsingForce (HandPresence hand) {
        if (hand.SecondaryButtonPressed)
            return true;

        return false;
    }

    bool CheckHandUsingFire (HandPresence hand) {
        if (hand.PrimaryButtonPressed)
            return true;

        return false;
    }


    private void SpawnFireball () {
        GameObject instans = Instantiate(FireballPrefab);
        instans.transform.position = _pointBetween;
        _castFireball = instans.GetComponent<Fireball>();
    }
}
