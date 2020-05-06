using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour {
    private InputDevice targetDevice;


    // Start is called before the first frame update
    void Start () {

        List<InputDevice> inputDevices = new List<InputDevice>();

        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;

        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, inputDevices);

        foreach (var item in inputDevices) {
            Debug.Log(item.name + item.characteristics);
        }

        if (inputDevices.Count > 0) {
            targetDevice = inputDevices [ 0 ];
        }

    }

    // Update is called once per frame
    void Update () {
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if (primaryButtonValue) {
            Debug.Log("Presing primary button");
        }
    }
}
