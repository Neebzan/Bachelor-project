using System;
using System.Collections;
using System.Collections.Generic;

using System.Threading;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine.PlayerLoop;

public class EmulatedFireball : MonoBehaviour {
    private Material _material;
    private readonly string _sizeProperty = "Vector1_EE3E2A7D";
    private float _size = 0.0f;
    public GameObject ParticleSystem;
    public int ID { get; private set; }

    private Vector3 _oldPos;
    private Vector3 _newPos;
    private Vector3 _velocity;

    private DateTime _lastTick;
    private float _tickTime;
    private Stopwatch stopwatch = new Stopwatch();

    void Awake () {
        _material = GetComponent<MeshRenderer>().material;
        transform.localScale = new Vector3(_size, _size, _size);
        //_material.SetFloat(_sizeProperty, _size);
    }

    public void Init (int id, float size) {
        ID = id;
        _size = size;
        //_tickTime = 0.0f;
        //stopwatch.Start();
    }

    public void Emulate (Vector3 position, float size) {
        //_tickTime = stopwatch.ElapsedMilliseconds;
        //stopwatch.Restart();

        //Debug.Log(_tickTime);

        //_oldPos = transform.position;
        transform.position = position;
        //_newPos = transform.position;
        //_velocity = (_newPos - _oldPos) / _tickTime;

        _size = size;

        transform.localScale = new Vector3(_size, _size, _size);
    }

    public void Despawn (bool explode) {
        if (explode) {
            var particles = GameObject.Instantiate(ParticleSystem, transform.position, Quaternion.identity).GetComponent<FireballExplosionParticles>();
            particles.Size = _size;
            //particles.Velocity = _velocity;
            Remove();
        }
        else {
            StartCoroutine(Shrink());
        }
    }


    IEnumerator Shrink () {
        while (_size > 0.0f) {
            _size -= Time.fixedDeltaTime * .1f;
            transform.localScale = new Vector3(_size, _size, _size);
            //_material.SetFloat(_sizeProperty, _size);
            yield return new WaitForFixedUpdate();
        }

        Remove();
    }

    private void Remove () {
        GameManager.EmulatedFireballs.Remove(ID);
        Destroy(gameObject);
    }
}
