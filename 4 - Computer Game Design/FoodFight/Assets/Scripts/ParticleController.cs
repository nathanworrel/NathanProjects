using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    ParticleSystem parry;
    ParticleSystem splatter;
    ParticleSystem pokeDamage;
    ParticleSystem swingDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayEffect(string name)
    {
        switch(name)
        {
            case "parry":
                parry.Play();
                break;
            case "splatter":
                splatter.Play();
                break;
            case "poke":
                pokeDamage.Play();
                break;
            case "swing":
                swingDamage.Play();
                break;
        } // switch
    }
}
