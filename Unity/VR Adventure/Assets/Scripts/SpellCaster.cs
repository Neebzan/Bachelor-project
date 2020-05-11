using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public class SpellCaster : MonoBehaviour {
    public GestureController LeftController;
    public GestureController RightController;


    private bool _isPressingRight = false;
    private bool _isPressingLeft = false;
    private bool castingSpell = false;

    public GameObject FireballPrefab;
    private CharacterController _characterController = null;
    private Fireball _castFireball;

    private Vector3 _pointBetween {
        get {
            return LeftController.PointerFinger.position + (RightController.PointerFinger.position - LeftController.PointerFinger.position) * .5f;
        }
    }


    void Start () {
        _characterController = GetComponent<CharacterController>();
    }

    void Update () {
        CheckForSpellcast();
        if (castingSpell) {
            _castFireball.transform.position = _pointBetween;
            float distance = Vector3.Distance(RightController.PointerFinger.position, LeftController.PointerFinger.position);
            _castFireball.Size = distance - .1f;
        }
    }


    private void CheckForSpellcast () {
        if (!castingSpell) {
            // Force Push
            if (RightController.HandMaterial != null) {
                if (RightController.CurrentGesture == HandGesture.Open) {
                    if (RightController.SecondaryButtonPressed) {
                        RightController.HandMaterial.SetFloat("Vector1_654486F", 1.0f);
                        RightController.HandMaterial.SetFloat("_Glow", 1.0f);
                    }
                }
            }

            // Shield
            if (RightController.CurrentGesture == HandGesture.Open && LeftController.CurrentGesture == HandGesture.Open) {

            }
        }

        if (!castingSpell && RightController.CurrentGesture == HandGesture.Pinch && LeftController.CurrentGesture == HandGesture.Pinch) {
            castingSpell = true;
            CastFireball();
        }

        else if (castingSpell && (RightController.CurrentGesture != HandGesture.Pinch || LeftController.CurrentGesture != HandGesture.Pinch)) {
            castingSpell = false;
            _castFireball.Create();
            _castFireball = null;
        }
    }

    private void CastFireball () {
        GameObject instans = Instantiate(FireballPrefab);
        instans.transform.position = _pointBetween;
        _castFireball = instans.GetComponent<Fireball>();
    }
}
