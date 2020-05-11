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

    private SphereCollider _collider;
    private Transform _pointerFinger;


    public bool SecondaryButtonPressed;

    private Material _handMaterial;
    public Material HandMaterial {
        get {
            if (_handMaterial == null) {
                if (RightHand)
                    _handMaterial = GameObject.Find("hands:Rhand")?.GetComponent<SkinnedMeshRenderer>()?.material;
                else
                    _handMaterial = GameObject.Find("hands:Lhand")?.GetComponent<SkinnedMeshRenderer>()?.material;
            }
            return _handMaterial;
        }
    }


    [HideInInspector]
    public Transform PointerFinger {
        get {
            if (_pointerFinger == null) {
                if (RightHand) {
                    _pointerFinger = GameObject.Find("hands:b_r_index_ignore")?.transform;
                }
                else
                    _pointerFinger = GameObject.Find("hands:b_l_index_ignore")?.transform;
            }

            return _pointerFinger == null ? transform : _pointerFinger;
        }
    }

    [HideInInspector]
    public HandGesture CurrentGesture { get; private set; }

    private Vector3 _velocity;
    private Vector3 _velocity2;


    private void Awake () {
        _collider = GetComponent<SphereCollider>();
    }

    private void Update () {
        CheckInput();
    }

    private void CheckInput () {
        if (Controller.enableInputActions) {
            CurrentGesture = HandGesture.Open;
            SecondaryButtonPressed = false;

            // Grisp
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
                _velocity = Quaternion.Euler(0, -90, 0) * velocityValue;
            }

            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue)) {
                SecondaryButtonPressed = secondaryButtonValue;
            }
        }
    }

    private void OnTriggerEnter (Collider other) {
        Debug.Log("On Trigger Enter");
        Fireball fireball = other.GetComponent<Fireball>();
        if (fireball != null) {
            if (fireball.Active) {
                //Fire fireball
                if (!fireball.Launched) {
                    Debug.Log("Fireball not launched");
                    if (CurrentGesture == HandGesture.Punch) {
                        Debug.Log("Launch fireball");
                        fireball.Launch(_velocity * 50.0f);
                    }
                }

                //Get hit
            }
        }
    }
}
