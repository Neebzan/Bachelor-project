using Boo.Lang;
using JetBrains.Annotations;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    public LayerMask ExplodeOnCollide;
    public static int IdIndexer = 0;
    public static readonly float FireballMinSize = 0.05f;
    public static readonly float FireballMaxSize = 0.2f;
    public static readonly float LargeFireballMaxSize = 0.7f;

    public int PlayerID;
    private static readonly float FireballGrowthRate = 1.0f;
    private SphereCollider _physicalCollider;
    private Rigidbody _rigidBody;
    private float sizePercentage {
        get {
            return ((Size - FireballMinSize) / (LargeFireballMaxSize - FireballMinSize));
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

    public void Create () {
        _physicalCollider.radius = Size * .5f;

        Active = true;
        Armed = true;

        ApplyForce(FireballVelocity * 3.5f);
    }

    public void SetSize (float targetSize) {
        float newSize = Mathf.Lerp(Size, targetSize, FireballGrowthRate * Time.deltaTime);

        float changeInSize = Mathf.Clamp(newSize - Size, -(1.0f * Time.deltaTime), 0.5f * Time.deltaTime);

        Size = Mathf.Clamp(Size + changeInSize, 0, LargeFireballMaxSize);
    }

    public void FollowTarget (Vector3 target) {
        Vector3 oldPos = transform.position;
        transform.position = Vector3.Lerp(transform.position, target, 0.05f + 0.1f * (1 - sizePercentage) * .1f);
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
                else if (other.tag == "Player") {
                    Console.WriteLine("Hit player");
                    Player hitPlayer = other.transform.root.gameObject.GetComponent<Player>();
                    if (hitPlayer.ID != this.PlayerID) {
                        Console.WriteLine("Player ID: " + hitPlayer.ID);
                        Console.WriteLine("Fireball ID: " + this.PlayerID);
                        destroy = true;

                        // If the player who cast the fireball exists on the server
                        if (Server.clients.ContainsKey(this.ID)) {
                            if (other.gameObject.name == "HeadCollider") {
                                // Hit head
                                Hit(hitPlayer, true);
                            }
                            else {
                                // Hit body
                                Hit(hitPlayer, false);
                            }
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

    private void Hit (Player hitPlayer, bool head) {
        Server.clients [ this.ID ].player.Score += head ? 3 : 1;

        if (Server.clients.ContainsKey(hitPlayer.ID))
            Console.WriteLine($"Player {Server.clients [ this.ID ].player.Username} Fireball → {hitPlayer.Username}'s  {(head ? "head" : "body")}");   
    }

    public void Despawn (bool explode) {
        ServerPacketSender.DespawnFireball(ID, explode);
        Destroy(this.gameObject);
    }
}
