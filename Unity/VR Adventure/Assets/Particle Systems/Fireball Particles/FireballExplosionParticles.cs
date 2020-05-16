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

    // Start is called before the first frame update
    void Start()
    {
        int numOfParticles = (int)(Size * 100.0f);

        //var velocityParamters = new ParticleSystem.EmitParams() { velocity = Velocity };

        TrailParticleSystem.Emit(numOfParticles);
        //TrailParticleSystem.transform.localScale = new Vector3(Size, Size, Size);
        SphereParticleSystem.transform.localScale = new Vector3(Size, Size, Size);
        SphereParticleSystem.Emit(numOfParticles);

        //particleSystem.shape.radius = Size;
        StartCoroutine(DestroyAfterPlay());
    }    

    IEnumerator DestroyAfterPlay () {
        while (TrailParticleSystem.particleCount > 0 && SphereParticleSystem.particleCount > 0) {
            yield return new WaitForSeconds(.1f);
        }

        Destroy(this.gameObject);
    }
}
