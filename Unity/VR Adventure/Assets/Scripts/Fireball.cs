using Boo.Lang;
using JetBrains.Annotations;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(MeshRenderer), typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    public LayerMask ExplodeOnCollide;

    private string _sizeProperty = "Vector1_EE3E2A7D";
    private string _wobbleSpeedProperty = "Vector1_27B1464A";
    private string _wobbleSizeProperty = "Vector1_417B252A";

    private MeshRenderer _meshRenderer;
    private SphereCollider _physicalCollider;
    private Rigidbody _rigidBody;

    public static int IdIndexer = 0;
    public int ID { get; private set; }

    [HideInInspector]
    public bool Active = false;
    [HideInInspector]
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

    bool isServer = true;

    private void Awake () {
        ID = IdIndexer++;
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidBody = GetComponent<Rigidbody>();
        _physicalCollider = GetComponent<SphereCollider>();
        if (isServer) {
            Init();
        }
    }

    void Init () {
        ServerPacketSender.SpawnFireball(this);
    }

    private void FixedUpdate () {
        if (isServer) {
            ServerPacketSender.UpdateFireball(this);
        }
    }

    private void Expand () {
        _meshRenderer.material.SetFloat(_sizeProperty, Size);
    }

    public void Create (Vector3 velocity) {
        _physicalCollider.radius = Size * .5f;
        Active = true;

        if (velocity.magnitude > .3f) {
            Armed = true;
            ApplyForce(velocity * 1.5f);
        }
    }

    public void FollowTarget (Vector3 target) {
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
                ServerPacketSender.DespawnFireball(ID);
                Destroy(this.gameObject);
            }
        }
    }
}
