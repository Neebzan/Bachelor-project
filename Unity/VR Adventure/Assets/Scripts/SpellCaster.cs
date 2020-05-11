using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public class SpellCaster : MonoBehaviour {
    public GestureController LeftController;
    public GestureController RightController;


    private bool _isPressingRight = false;
    private bool _isPressingLeft = false;
    private bool castingSpell = false;

    public GameObject FireballPrefab;
    private CharacterController _characterController = null;
    private Fireball _castFireball;

    void Start () {
        _characterController = GetComponent<CharacterController>();
    }

    void Update () {
        CheckForSpellcast();
        if (castingSpell) {
            Vector3 pointBetween = LeftController.transform.position + (RightController.transform.position - LeftController.transform.position) * .5f;
            _castFireball.transform.position = pointBetween;
            _castFireball.Size = Vector3.Distance(RightController.transform.position, LeftController.transform.position) * .5f;
        }
    }


    private void CheckForSpellcast () {
        if (!castingSpell && RightController.CurrentGesture == HandGesture.Pinch && LeftController.CurrentGesture == HandGesture.Pinch) {
            castingSpell = true;
            CastFireball();
        }

        else if (castingSpell && (RightController.CurrentGesture != HandGesture.Pinch || LeftController.CurrentGesture != HandGesture.Pinch)) {
            castingSpell = false;
            _castFireball.Create();
            _castFireball = null;
        }
    }

    private void CastFireball () {
        Vector3 pointBetween = LeftController.transform.position + (RightController.transform.position - LeftController.transform.position) * .5f;

        GameObject instans = Instantiate(FireballPrefab);
        instans.transform.position = pointBetween;
        _castFireball = instans.GetComponent<Fireball>();
    }
}
