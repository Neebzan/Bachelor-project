using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public enum HandGesture { Open, Pinch, Punch, Grip }
public class HandPresence : MonoBehaviour {

    [HideInInspector]
    public HandGesture CurrentGesture { get; private set; }
    private InputDevice targetDevice;
    public XRController Controller;
    public bool SecondaryButtonPressed;

    public Vector3 Velocity;
    [HideInInspector]
    public Hand Hand;

    private Animator handAnimator;

    void Start () {
        targetDevice = Controller.inputDevice;
        handAnimator = GetComponent<Animator>();
        Hand = GetComponent<Hand>();
    }

    void UpdateHandAnimation () {
        CurrentGesture = HandGesture.Open;
        SecondaryButtonPressed = false;


        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
            handAnimator.SetFloat("Grip", gripValue);
            CurrentGesture = gripValue > .5f ? HandGesture.Grip : HandGesture.Open;
        }
        else {
            handAnimator.SetFloat("Grip", 0);
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) {
            handAnimator.SetFloat("Trigger", triggerValue);
            if (triggerValue > .5f) {
                CurrentGesture = CurrentGesture == HandGesture.Grip ? HandGesture.Punch : HandGesture.Pinch;
            }
        }
        else {
            handAnimator.SetFloat("Trigger", 0);
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocityValue)) {
            Velocity = Quaternion.Euler(0, -90, 0) * velocityValue;
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue)) {
            SecondaryButtonPressed = secondaryButtonValue;
        }
    }


    void Update () {
        UpdateHandAnimation();
    }
}
