using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireballExplosionParticles : MonoBehaviour
{
    ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(DestroyAfterPlay());
    }    

    IEnumerator DestroyAfterPlay () {
        while (particleSystem.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }

        Destroy(this.gameObject);
    }
}
