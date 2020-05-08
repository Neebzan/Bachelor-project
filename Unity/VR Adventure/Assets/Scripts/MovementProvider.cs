using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class MovementProvider : LocomotionProvider {
    public List<XRController> Controllers = null;
    public float Speed = 1.0f;
    public float GravityMultiplier = 1.0f;

    private CharacterController _characterController = null;
    private GameObject _head = null;


    private void Awake () {
        _characterController = GetComponent<CharacterController>();
        _head = GetComponent<XRRig>().cameraGameObject;
    }

    void Start () {
        PositionController();
    }

    void Update () {
        PositionController();
        CheckForInput();
        ApplyGravity();
    }

    private void PositionController () {
        // Get the head position in localspace, set that height to the character controller
        float headHeight = Mathf.Clamp(_head.transform.localPosition.y, 1, 2);
        _characterController.height = headHeight;

        // Take half the height, and add the skinwidth to it
        Vector3 newCenter = Vector3.zero;
        newCenter.y = _characterController.height / 2;
        newCenter.y += _characterController.skinWidth;

        // Set the x-z position to match the heads position
        newCenter.x = _head.transform.localPosition.x;
        newCenter.z = _head.transform.localPosition.z;

        // Apply
        _characterController.center = newCenter;
    }

    private void CheckForInput () {
        foreach (XRController controller in Controllers) {
            if (controller.enableInputActions)
                CheckForMovement(controller.inputDevice);
        }
    }

    private void CheckForMovement (InputDevice inputDevice) {
        if (inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue)) {
            StartMove(primary2DAxisValue);
        }
    }

    private void StartMove (Vector2 position) {
        // Apply touch position to the head's forward vector
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, _head.transform.eulerAngles.y, 0);

        direction = Quaternion.Euler(headRotation) * direction;

        Vector3 movement = direction * Speed;
        _characterController.Move(movement * Time.deltaTime);
    }

    private void ApplyGravity () {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * GravityMultiplier, 0);
        gravity.y *= Time.deltaTime;

        _characterController.Move(gravity * Time.deltaTime);
    }
}
