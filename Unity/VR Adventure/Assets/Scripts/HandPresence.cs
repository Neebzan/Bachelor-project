using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresence : MonoBehaviour {
    public XRController Controller;

    private InputDevice _targetDevice;
    private Animator _handAnimator;
    private Transform _rootParent;

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
        _rootParent = transform.root;
    }

    void UpdateHandInputs () {
        CurrentGesture = HandGesture.Open;

        _targetDevice.TryGetFeatureValue(CommonUsages.grip, out GripValue);
        _targetDevice.TryGetFeatureValue(CommonUsages.trigger, out TriggerValue);
        _targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out SecondaryButtonPressed);
        _targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out PrimaryButtonPressed);
        

        if (_targetDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 value)) {
            //Velocity = Quaternion.Euler(0, -90, 0) * value;
            Velocity = _rootParent.rotation * value;
        }
    }

    void UpdateHandAnimation () {
        _handAnimator.SetFloat("Grip", GripValue);
        _handAnimator.SetFloat("Trigger", TriggerValue);
    }

    void UpdateHandGesture () {
        CurrentGesture = GripValue > .5f ? HandGesture.Grip : TriggerValue > .5f ? HandGesture.Pinch : HandGesture.Open;
    }

    void Update () {
        UpdateHandInputs();
        UpdateHandAnimation();
        UpdateHandGesture();
    }
}
