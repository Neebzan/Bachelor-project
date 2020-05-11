using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour {
    private InputDevice targetDevice;
    public GameObject HandModelPrefab;
    public InputDeviceCharacteristics controllerCharacteristics;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    // Start is called before the first frame update
    void Start () {

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, inputDevices);

        //foreach (var item in inputDevices) {
        //    Debug.Log(item.name + item.characteristics);
        //}

        if (inputDevices.Count > 0) {
            targetDevice = inputDevices [ 0 ];
        }

        spawnedHandModel = GameObject.Instantiate(HandModelPrefab, transform);
        handAnimator = spawnedHandModel.GetComponent<Animator>();

    }

    void UpdateHandAnimation () {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else {
            handAnimator.SetFloat("Grip", 0);
        }
    }




    // Update is called once per frame
    void Update () {
        PointFinger();


        UpdateHandAnimation();


        //if(targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
        //    Debug.Log("Presing primary button");

        //if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1)
        //    Debug.Log("Trigger pressed: " + triggerValue);

        //if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue) && primary2DAxisValue != Vector2.zero)
        //    Debug.Log("Primary touchpad: " + primary2DAxisValue);


    }

    private void PointFinger () {

        //RaycastHit hit;

        //GraphicRaycaster.

        //if(Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 100.0f)) {
        //    hit.
        //}

        

    }
}
