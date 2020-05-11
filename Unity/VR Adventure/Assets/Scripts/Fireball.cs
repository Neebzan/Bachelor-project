using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(MeshRenderer), typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    private string _sizeProperty = "Vector1_EE3E2A7D";
    private string _wobbleSpeedProperty = "Vector1_27B1464A";
    private string _wobbleSizeProperty = "Vector1_417B252A";

    private MeshRenderer _meshRenderer;
    private SphereCollider _collider;
    private Rigidbody _rigidBody;

    public bool Active = false;
    public bool Launched = false;

    private float _size;
    public float Size {
        get {
            return _size;
        }

        set {
            _size = value;
            Expand();
        }
    }

    private void Awake () {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<SphereCollider>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Expand () {
        _meshRenderer.material.SetFloat(_sizeProperty, Size);
        //_meshRenderer.material.SetFloat(_wobbleSizeProperty, Size * .5f);
    }

    public void Create () {
        _collider.radius = Size * .5f;
        Active = true;
    }

    public void Launch (Vector3 relativeVelocity) {
        if (!Launched) {
            _rigidBody.AddForce(relativeVelocity);
            Launched = true;
        }
    }

    //private void OnTriggerEnter (Collider other) {
    //    if (other.tag == "Controller") {
    //    Debug.Log("collision hit!");
    //    //    Fireball fireball = collision.gameObject.GetComponent<Fireball>();
    //    //    if (fireball != null) {
    //    //        if (fireball.Active) {
    //    //            //Fire fireball
    //    //            if (!fireball.Launched) {
    //    //                if (CurrentGesture == HandGesture.Punch) {
    //    //                    fireball.Launch(collision.relativeVelocity);
    //    //                }
    //    //            }

    //    //            //Get hit
    //    //        }
    //    //    }
    //    //}
    //}
}
