using System;
using System.Collections;
using System.Collections.Generic;

using System.Threading;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine.PlayerLoop;

public class EmulatedFireball : MonoBehaviour {
    private float _size = 0.0f;
    public GameObject ParticleSystem;

    public int ID { get; private set; }

    public long _lastUpdateTick = 0;

    void Awake () {
        transform.localScale = new Vector3(_size, _size, _size);
    }

    public void Init (int id, float size) {
        ID = id;
        _size = size;
    }

    public void Emulate (Vector3 position, float size) {
        transform.position = position;
        _size = size;
        transform.localScale = new Vector3(_size, _size, _size);
    }

    public void Despawn (bool explode) {
        if (explode) {
            var particles = GameObject.Instantiate(ParticleSystem, transform.position, Quaternion.identity).GetComponent<FireballExplosionParticles>();
            particles.Size = _size;
            Remove();
        }
        else {
            StartCoroutine(Shrink());
        }
    }


    IEnumerator Shrink () {
        while (_size > 0.0f) {
            _size -= Time.fixedDeltaTime * .3f;
            transform.localScale = new Vector3(_size, _size, _size);
            yield return new WaitForFixedUpdate();
        }

        Remove();
    }

    private void Remove () {
        GameManager.EmulatedFireballs.Remove(ID);
        Destroy(gameObject);
    }
}
