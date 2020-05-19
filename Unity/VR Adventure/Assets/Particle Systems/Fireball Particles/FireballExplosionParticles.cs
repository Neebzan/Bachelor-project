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

    void Start()
    {
        // Base number of particles to spawn
        int numOfParticles = (int)(Size * 100.0f);

        // Sparks in explosion
        TrailParticleSystem.Emit(numOfParticles * 2);

        // Spheres in explosion
        SphereParticleSystem.Emit(numOfParticles);
        SphereParticleSystem.transform.localScale = new Vector3(Size * .5f, Size * .5f, Size * .5f);

        // Begin check for destroy after play
        StartCoroutine(DestroyAfterPlay());
    }    

    IEnumerator DestroyAfterPlay () {
        while (TrailParticleSystem.particleCount > 0 && SphereParticleSystem.particleCount > 0) {
            yield return new WaitForSeconds(.1f);
        }

        Destroy(this.gameObject);
    }
}
