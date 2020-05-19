using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballExplosionParticles : MonoBehaviour
{

    public ParticleSystem TrailParticleSystem;
    public ParticleSystem SphereParticleSystem;
    public float Size { get; set; }
    public Vector3 Velocity { get; set; }

    private void Awake () {

    }

    void Start()
    {
        // Base number of particles to spawn
        int baseNumOfParticles = (int)(Size * 100.0f);

        // Sparks in explosion
        TrailParticleSystem.Emit(baseNumOfParticles);
        SphereParticleSystem.transform.localScale = new Vector3(Size, Size, Size);

        // Spheres in explosion
        //SphereParticleSystem.Emit(baseNumOfParticles / 2);


        // Begin check for destroy after play
        StartCoroutine(DestroyAfterPlay());
    }    

    IEnumerator DestroyAfterPlay () {
        while (TrailParticleSystem.particleCount > 0 || SphereParticleSystem.particleCount > 0) {
            yield return new WaitForSeconds(.1f);
        }

        Destroy(this.gameObject);
    }
}
