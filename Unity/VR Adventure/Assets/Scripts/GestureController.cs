using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum HandGesture { Open, Pinch, Punch, Grip }
public class GestureController : MonoBehaviour {
    public XRController Controller;
    public bool RightHand = true;

    public SphereCollider SphereCollider;
    private SphereCollider _collider;
    private Transform _pointerFinger;


    private Hand _hand;
    public Hand Hand {
        get {
            if (_hand == null) {
                Transform modelTransform = Controller?.modelTransform;
                _hand = modelTransform.childCount > 0 ? modelTransform.GetChild(0).childCount > 0 ? modelTransform.GetChild(0).GetChild(0).GetComponent<Hand>() : null : null;
            }
            return _hand;
        }
    }

    public bool SecondaryButtonPressed;


    [HideInInspector]
    public HandGesture CurrentGesture { get; private set; }

    public Vector3 Velocity;
    private Vector3 _velocity2;


    private void Awake () {
        _collider = GetComponent<SphereCollider>();
    }

    private void Update () {
        CheckInput();
    }

    private void CheckInput () {
        if (Controller.enableInputActions) {
            // Resetting
            CurrentGesture = HandGesture.Open;
            SecondaryButtonPressed = false;

            // Grasp
            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
                CurrentGesture = gripValue > .5f ? HandGesture.Grip : HandGesture.Open;
            }

            // Pinch & Punch
            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue)) {
                if (triggerValue) {
                    CurrentGesture = CurrentGesture == HandGesture.Grip ? HandGesture.Punch : HandGesture.Pinch;
                }
            }

            // Velocity
            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocityValue)) {
                Velocity = Quaternion.Euler(0, -90, 0) * velocityValue;
            }

            // Secondary button
            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue)) {
                SecondaryButtonPressed = secondaryButtonValue;
            }
        }
    }
}
