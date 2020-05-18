using Boo.Lang;
using JetBrains.Annotations;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    public LayerMask ExplodeOnCollide;

    private SphereCollider _physicalCollider;
    private Rigidbody _rigidBody;

    public static int IdIndexer = 0;
    public static readonly float FireballMinSize = 0.05f;
    public static readonly float FireballMaxSize = 0.4f;
    private static readonly float FireballGrowthRate = 1.0f;

    private float sizePercentage {
        get {
            return ((Size-FireballMinSize) / (FireballMaxSize-FireballMinSize));
        }
    }

    public int ID { get; private set; }

    public Vector3 FireballVelocity { get; set; }

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
        }
    }

    bool isServer = true;

    private void Awake () {
        ID = IdIndexer++;
        _rigidBody = GetComponent<Rigidbody>();
        _physicalCollider = GetComponent<SphereCollider>();
        if (isServer) {
            Init();
        }
    }

    void Init () {
        ServerPacketSender.SpawnFireball(this);
    }

    public void Create (Vector3 velocity) {
        _physicalCollider.radius = Size * .5f;

        Active = true;
        Armed = true;

        if (velocity.magnitude > .3f) {
            //ApplyForce(velocity * 1.5f);
            ApplyForce(FireballVelocity * 1.5f);
        }
    }

    public void SetSize (float targetSize) {
        float newSize = Mathf.Lerp(Size, targetSize, FireballGrowthRate * Time.deltaTime);

        float changeInSize = Mathf.Clamp(newSize - Size, -(1.0f * Time.deltaTime), 0.5f * Time.deltaTime);

        Size += changeInSize;
    }

    public void FollowTarget (Vector3 target) {
        Vector3 oldPos = transform.position;
        transform.position = Vector3.Lerp(transform.position, target, 0.05f + 0.1f * (1 - sizePercentage) * .5f);
        Vector3 newPos = transform.position;

        FireballVelocity = (newPos - oldPos) / Time.fixedDeltaTime;
    }

    public void ApplyForce (Vector3 relativeVelocity) {
        _rigidBody.velocity = relativeVelocity;
    }

    public void Launch (Vector3 relativeVelocity) {
        ApplyForce(relativeVelocity);
        Armed = true;
    }

    private void OnTriggerEnter (Collider other) {
        bool destroy = false;
        if (Armed) {
            if (ExplodeOnCollide == (ExplodeOnCollide | (1 << other.gameObject.layer))) {
                if (other.tag == "Projectile") {
                    Fireball otherFireball = other.GetComponent<Fireball>();
                    if (otherFireball != null) {
                        if (otherFireball.Armed) {
                            destroy = true;
                        }
                    }
                }
                else
                    destroy = true;
            }
        }

        if (destroy)
            Despawn(true);
    }

    public void Despawn (bool explode) {
        ServerPacketSender.DespawnFireball(ID, explode);
        Destroy(this.gameObject);
    }
}
