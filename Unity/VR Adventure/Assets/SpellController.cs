using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    [HideInInspector]
    public float ForceChargeupMax = 1.0f;
    [HideInInspector]
    public float ForceCharge = 0.0f;
    [HideInInspector]
    public float ForcePercentageCharged = 0.0f;
    [HideInInspector]
    public bool UsingForce = false;
    
    private Material _handMaterial;
    private SphereCollider _collider;

    private void Start () {
        _handMaterial = GetComponent<Hand>().SkinnedMeshRenderer.material;
        _collider = GetComponent<SphereCollider>();
    }

    private void Update () {
        if (UsingForce) {
            if (ForceCharge < 1.0f) {
                ChargeForce();
            }
        }
        else {
            if (ForceCharge > 0.0f) {
                DechargeForce();
            }
        }
    }

    public void ChargeForce () {
        if (ForceCharge < ForceChargeupMax) {
            ForceCharge += Time.deltaTime;
            ForceCharge = Mathf.Clamp(ForceCharge, 0, ForceChargeupMax);
            ForcePercentageCharged = ForceCharge / ForceChargeupMax;
            _handMaterial.SetFloat("Vector1_654486F", ForcePercentageCharged);
            Debug.Log(ForcePercentageCharged);
        }
    }

    public void DechargeForce () {
        ForceCharge -= Time.deltaTime;
        ForceCharge = Mathf.Clamp(ForceCharge, 0, ForceChargeupMax);
        ForcePercentageCharged = ForceCharge / ForceChargeupMax;
        _handMaterial.SetFloat("Vector1_654486F", ForcePercentageCharged);
    }

    //private void OnTriggerEnter (Collider other) {
    //    if (other.gameObject.layer) {

    //    }
    //}

}
