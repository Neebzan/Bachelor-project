using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class UIProvider : MonoBehaviour {
    public List<XRDirectInteractor> DirectInteractors = null;
    public List<XRRayInteractor> RayInteractors = null;
    public List<XRController> Controllers = null;
    public bool StartInMenu = false;
    public GameObject UI;

    private bool _menuOpened = false;
    private bool _isPressingButton = false;
    private CharacterController _characterController = null;

    void Start () {
        SetMenu(StartInMenu);

        _characterController = GetComponent<CharacterController>();
    }

    void Update () {
        CheckInput();
    }

    private void CheckInput () {
        foreach (XRController controller in Controllers) {
            if (controller.enableInputActions)  
                CheckForMenuOpen(controller.inputDevice);
            
        }
    }

    private void CheckForMenuOpen (InputDevice inputDevice) {
        if (inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonPressed)) {
            if (!_isPressingButton && primaryButtonPressed) {
                _isPressingButton = true;
                Debug.Log("Primary button pressed");
                SetMenu(!_menuOpened);
            }
            else if (_isPressingButton && !primaryButtonPressed) {
                _isPressingButton = false;
            }
        }
    }

    private void SetMenu (bool value) {
        _menuOpened = value;
        UI.SetActive(value);
        foreach (var interactor in DirectInteractors) {
            interactor.gameObject.SetActive(!value);
        }
        foreach (var interactor in RayInteractors) {
            interactor.gameObject.SetActive(value);
        }
    }
}
