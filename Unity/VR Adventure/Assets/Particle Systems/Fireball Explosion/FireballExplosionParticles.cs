using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireballExplosionParticles : MonoBehaviour
{
    ParticleSystem particleSystem;
    public float Size { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        //particleSystem.main.

        //particleSystem.shape.radius = Size;
        StartCoroutine(DestroyAfterPlay());
    }    

    IEnumerator DestroyAfterPlay () {
        while (particleSystem.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }

        Destroy(this.gameObject);
    }
}
