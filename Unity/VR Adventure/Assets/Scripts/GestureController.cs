using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum HandGesture { Open, Pinch, Punch, Grip }
public class GestureController : MonoBehaviour {
    public XRController Controller;

    private SphereCollider _collider;

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

            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
                CurrentGesture = gripValue > .5f ? HandGesture.Grip : HandGesture.Open;
            }

            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue)) {
                if (triggerValue) {
                    CurrentGesture = CurrentGesture == HandGesture.Grip ? HandGesture.Punch : HandGesture.Pinch;
                }
            }

            if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocityValue)) {
                //_velocity = Quaternion.AngleAxis(90, Vector3.up) * velocityValue;
                _velocity = Quaternion.Euler(0, -90, 0) * velocityValue;
                //_velocity2 = transform.TransformVector(velocityValue);
            }
        }
    }

    //private void OnDrawGizmos () {
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(Controller.transform.position, Controller.transform.position + _velocity.normalized * 2.0f);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(Controller.transform.position, Controller.transform.position + _velocity2.normalized * 2.0f);

    //}

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
