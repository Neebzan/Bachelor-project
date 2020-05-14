using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(MeshRenderer), typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    private string _sizeProperty = "Vector1_EE3E2A7D";
    private string _wobbleSpeedProperty = "Vector1_27B1464A";
    private string _wobbleSizeProperty = "Vector1_417B252A";

    private MeshRenderer _meshRenderer;


    private SphereCollider _physicalCollider;
    public LayerMask ExplodeOnCollide;

    private Rigidbody _rigidBody;

    public bool Active = false;
    public bool Armed = false;

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
        _rigidBody = GetComponent<Rigidbody>();
        _physicalCollider = GetComponent<SphereCollider>();
    }

    private void Expand () {
        _meshRenderer.material.SetFloat(_sizeProperty, Size);
    }



    public void Create (Vector3 rightControllerVelocity, Vector3 leftControllerVelocity) {
        _physicalCollider.radius = Size * .5f;
        Active = true;

        Vector3 velocity = rightControllerVelocity + leftControllerVelocity;
        if (velocity.magnitude > 1.0f) {
            Armed = true;
            ApplyForce(velocity * .5f);
        }
    }

    public void FollowTarget(Vector3 target) {
            Vector3 between = target - transform.position;
           transform.position = Vector3.Lerp(transform.position, target, 0.1f);        
    }

    public void ApplyForce (Vector3 relativeVelocity) {
        _rigidBody.velocity = relativeVelocity;
    }

    public void Launch (Vector3 relativeVelocity) {
        ApplyForce(relativeVelocity);
        Armed = true;
    }


    private void OnTriggerEnter (Collider other) {
        if (Armed) {
            if (ExplodeOnCollide == (ExplodeOnCollide | (1 << other.gameObject.layer))) {
                Destroy(this.gameObject);
            }
        }
    }
}
