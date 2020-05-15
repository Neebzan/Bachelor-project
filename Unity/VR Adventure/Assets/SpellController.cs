using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpellController : MonoBehaviour {
    public HandPresence Hand;
    public LayerMask ProjectileMask;


    private float _forceChargeupMax = 1.0f;
    public float ForceCharge = 0.0f;
    public float ForcePercentageCharged = 0.0f;

    private float _fireChargeupMax = 1.0f;
    public float FireCharge = 0.0f;
    public float FirePercentageCharged = 0.0f;

    public float StatePower { get; private set; }

    private HandState _handState;
    public HandState HandState {
        get { return _handState; }
        set {
            if (value == HandState.Fire) {
                _handMaterial.SetColor(_glowColorId, FireColor);
            }
            else if (value == HandState.Force) {
                _handMaterial.SetColor(_glowColorId, ForceColor);
            }
            _handState = value;
        }
    }

    [HideInInspector]
    public HandState InputState;


    private float _forcePower = 2.5f;
    private float _minimumVelocity = 0.0f;
    private Material _handMaterial;
    private SphereCollider _collider;

    private string _glowIntensityId = "Vector1_654486F";
    private string _glowColorId = "Color_F07FA635";

    [ColorUsage(true, true)]
    public Color FireColor;
    [ColorUsage(true, true)]
    public Color ForceColor;




    private void Start () {
        _handMaterial = GetComponent<Hand>().SkinnedMeshRenderer.material;
        _collider = GetComponent<SphereCollider>();
    }

    internal void UpdateHandState (HandPresence controller) {
        InputState = HandState.Default;
        if (controller.SecondaryButtonPressed) {
            InputState = HandState.Force;
            if (HandState != HandState.Force) {
                if (FirePercentageCharged > 0) {
                    DechargePower(ref FireCharge, ref _fireChargeupMax, ref FirePercentageCharged);
                }
                else {
                    HandState = HandState.Force;
                }
            }
            else if (HandState == HandState.Force && ForcePercentageCharged < 1.0f) {
                ChargePower(ref ForceCharge, ref _forceChargeupMax, ref ForcePercentageCharged);
            }
        }


        else if (controller.PrimaryButtonPressed) {
            InputState = HandState.Fire;
            if (HandState != HandState.Fire) {
                if (ForcePercentageCharged > 0) {
                    DechargePower(ref ForceCharge, ref _forceChargeupMax, ref ForcePercentageCharged);
                }
                else {
                    HandState = HandState.Fire;
                }
            }
            else if (HandState == HandState.Fire && FirePercentageCharged < 1.0f) {
                ChargePower(ref FireCharge, ref _fireChargeupMax, ref FirePercentageCharged);
            }
        }


        else {
            if (ForcePercentageCharged != 0.0f) {
                DechargePower(ref ForceCharge, ref _forceChargeupMax, ref ForcePercentageCharged);
                //decharge force
            }
            if (FirePercentageCharged != 0.0f) {
                //decharge fire
                DechargePower(ref FireCharge, ref _fireChargeupMax, ref FirePercentageCharged);
            }
        }

        if (HandState == HandState.Fire) {
            StatePower = FirePercentageCharged;
        }
        else if (HandState == HandState.Force) {
            StatePower = ForcePercentageCharged;
        }
    }

    //private void Update () {
    //    switch (HandState) {
    //        case HandState.Default:
    //            if (FireCharge != 0) {
    //                DechargePower(ref FireCharge, ref _fireChargeupMax, ref FirePercentageCharged);
    //            }
    //            if (ForceCharge != 0) {
    //                DechargePower(ref ForceCharge, ref _forceChargeupMax, ref ForcePercentageCharged);
    //            }

    //            break;


    //        case HandState.Fire:
    //            if (ForceCharge <= 0) {
    //                if (FireCharge < 1.0f) {
    //                    ChargePower(ref FireCharge, ref _fireChargeupMax, ref FirePercentageCharged);
    //                    StatePower = FirePercentageCharged;
    //                }
    //            }
    //            else
    //                DechargePower(ref ForceCharge, ref _forceChargeupMax, ref ForcePercentageCharged);
    //            break;


    //        case HandState.Force:
    //            if (FireCharge <= 0) {
    //                if (ForceCharge < 1.0f) {
    //                    ChargePower(ref ForceCharge, ref _forceChargeupMax, ref ForcePercentageCharged);
    //                    StatePower = ForcePercentageCharged;
    //                }
    //            }
    //            else
    //                DechargePower(ref FireCharge, ref _fireChargeupMax, ref FirePercentageCharged);

    //            break;
    //        default:
    //            break;
    //    }

    //    if (ForceCharge > 0) {
    //        _handMaterial.SetColor(_glowColorId, ForceColor);
    //    }
    //    else if (FireCharge > 0)
    //        _handMaterial.SetColor(_glowColorId, FireColor);
    //}


    public void ChargePower (ref float charge, ref float maxCharge, ref float percentageCharged) {
        if (charge < maxCharge) {
            charge += Time.deltaTime;
            charge = Mathf.Clamp(charge, 0, maxCharge);
            percentageCharged = charge / maxCharge;
            _handMaterial.SetFloat(_glowIntensityId, percentageCharged);
        }
    }

    public void DechargePower (ref float charge, ref float maxCharge, ref float percentageCharged) {
        charge -= Time.deltaTime;
        charge = Mathf.Clamp(charge, 0, maxCharge);
        percentageCharged = charge / maxCharge;
        _handMaterial.SetFloat(_glowIntensityId, percentageCharged);
    }
    private void OnTriggerEnter (Collider other) {
        if (ProjectileMask == (ProjectileMask | (1 << other.gameObject.layer))) {
            if (Hand.Velocity.magnitude > _minimumVelocity) {
                Fireball fireBall = other.gameObject.GetComponent<Fireball>();
                if (fireBall.Active) {
                    Vector3 force = Hand.Velocity * ForcePercentageCharged * _forcePower;

                    if (fireBall.Armed)
                        fireBall.ApplyForce(force);
                    else
                        fireBall.Launch(force);
                }
            }
        }
    }
}

