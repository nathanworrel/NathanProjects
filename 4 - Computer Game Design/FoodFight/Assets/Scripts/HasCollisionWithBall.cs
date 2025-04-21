using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasCollisionWithBall : MonoBehaviour
{
    HasHealth health;
    private int tomatoDMG = -1;
    public GameObject splatterParticles;
    ParticleSystem splatterParticleSystem;
    public AudioClip sound_clip;
    Vector3 sound_position;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HasHealth>();
        splatterParticleSystem = splatterParticles.GetComponent<ParticleSystem>();
        sound_position = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "ThrownObject")
        {
            Destroy(other.gameObject);
            health.changeHealth(tomatoDMG);
            splatterParticleSystem.Play();
            AudioSource.PlayClipAtPoint(sound_clip, sound_position);
        }   
    }

}

