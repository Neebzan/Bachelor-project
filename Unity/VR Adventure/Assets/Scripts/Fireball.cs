using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(MeshRenderer), typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    private string _sizeProperty = "Vector1_EE3E2A7D";
    private string _wobbleSpeedProperty = "Vector1_27B1464A";
    private string _wobbleSizeProperty = "Vector1_417B252A";

    private MeshRenderer _meshRenderer;

    [SerializeField]
    private SphereCollider _physicalCollider;
    [SerializeField]
    private SphereCollider _triggerCollider;

    private Rigidbody _rigidBody;

    public bool Active = false;

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
        _triggerCollider = GetComponent<SphereCollider>();
    }

    private void Expand () {
        _meshRenderer.material.SetFloat(_sizeProperty, Size);
    }

    public void Create () {
        _physicalCollider.radius = Size * .5f;
        _triggerCollider.radius = Size * .5f;
        Active = true;
    }

    public void ApplyForce (Vector3 relativeVelocity) {
        _rigidBody.AddForce(relativeVelocity);
    }
}
