using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GestureController : MonoBehaviour {



    //private void CheckInput () {
    //    if (Controller.enableInputActions) {
    //        // Resetting
    //        CurrentGesture = HandGesture.Open;
    //        SecondaryButtonPressed = false;

    //        // Grasp
    //        if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
    //            CurrentGesture = gripValue > .5f ? HandGesture.Grip : HandGesture.Open;
    //        }

    //        // Pinch & Punch
    //        if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue)) {
    //            if (triggerValue) {
    //                CurrentGesture = CurrentGesture == HandGesture.Grip ? HandGesture.Punch : HandGesture.Pinch;
    //            }
    //        }

    //        // Velocity
    //        if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocityValue)) {
    //            Velocity = Quaternion.Euler(0, -90, 0) * velocityValue;
    //        }

    //        // Secondary button
    //        if (Controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue)) {
    //            SecondaryButtonPressed = secondaryButtonValue;
    //        }
    //    }
    //}
}
