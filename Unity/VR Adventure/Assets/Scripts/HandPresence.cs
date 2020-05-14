using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public enum HandGesture { Open, Pinch, Punch, Grip }
public class HandPresence : MonoBehaviour {
    public XRController Controller;

    private InputDevice _targetDevice;
    private Animator _handAnimator;

    #region public fields
    [HideInInspector]
    public HandGesture CurrentGesture { get; private set; }

    [HideInInspector]
    public bool SecondaryButtonPressed;

    [HideInInspector]
    public bool PrimaryButtonPressed;

    [HideInInspector]
    public Vector3 Velocity;

    [HideInInspector]
    public Hand Hand;

    [HideInInspector]
    public float TriggerValue;

    [HideInInspector]
    public float GripValue;
    #endregion

    void Start () {
        _targetDevice = Controller.inputDevice;
        _handAnimator = GetComponent<Animator>();
        Hand = GetComponent<Hand>();
    }

    void UpdateHandInputs () {
        CurrentGesture = HandGesture.Open;

        _targetDevice.TryGetFeatureValue(CommonUsages.grip, out GripValue);
        _targetDevice.TryGetFeatureValue(CommonUsages.trigger, out TriggerValue);
        _targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out SecondaryButtonPressed);
        _targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out PrimaryButtonPressed);
        

        if (_targetDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 value)) {
            Velocity = Quaternion.Euler(0, -90, 0) * value;
        }
    }

    void UpdateHandAnimation () {
        _handAnimator.SetFloat("Grip", GripValue);
        _handAnimator.SetFloat("Trigger", TriggerValue);
    }

    void UpdateHandGesture () {
        CurrentGesture = GripValue > .5f ? HandGesture.Grip : HandGesture.Open;

        if (TriggerValue > .5f)
            CurrentGesture = CurrentGesture == HandGesture.Grip ? HandGesture.Punch : HandGesture.Pinch;
        
    }

    void Update () {
        UpdateHandInputs();
        UpdateHandAnimation();
        UpdateHandGesture();
    }
}
