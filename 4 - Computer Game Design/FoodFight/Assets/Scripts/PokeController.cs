using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeController : MonoBehaviour
{
    public GameObject self;
    public ParticleSystem takePokeDamageParticles;
    public GameObject parryParticles;

    ParticleSystem parryParticleSystem;
    PlayerAction selfAction;
    CameraShake cameraShake;
    BoscoStick boscoStick;
    float timer;
    float spaced_time = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        selfAction = self.GetComponent<PlayerAction>();
        cameraShake = self.GetComponentInChildren<CameraShake>();
        parryParticleSystem = parryParticles.GetComponent<ParticleSystem>();
        boscoStick = GetComponent<BoscoStick>();
        timer = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        collided(other);
    }

    private void OnTriggerStay(Collider other) {
        collided(other);
    }

    //TODO: Fix Camera Shake
    private void collided(Collider other){
        if (selfAction.playerAction == PlayerAction.Action.POKE
             && other.gameObject.CompareTag("Player")
             && other.gameObject != self 
             && timer < Time.time)
        {
            timer = Time.time + spaced_time;
            PlayerAction.Action opponentAction = other.gameObject.GetComponent<PlayerAction>().playerAction;
            Debug.Log(selfAction.playerAction + " " + opponentAction);
            switch (opponentAction)
            {
                case PlayerAction.Action.BLOCK: // poke blocking player (SELF WIN)
                                                // STUNNED 2 FOR LONG STUN. DO NOT CHANGE THIS 2 OR IT WILL CRASH.
                    other.transform.Find("BoscoStick").GetComponent<PokeController>().playParticles();
                    //other.gameObject.transform.Find("BoscoStick").gameObject.GetComponent<ParticleSystem>().Play();
                    StartCoroutine(other.gameObject.GetComponent<PlayerAction>().Stunned(2));
                    StartCoroutine(other.transform.Find("Player Camera").GetComponent<CameraShake>().Shake(true));
                    other.gameObject.GetComponent<PlayerAction>().BOOP();
                    takePokeDamageParticles.Play();
                    break;
                case PlayerAction.Action.BLOCK_IN:
                    // SAME AS BLOCK
                    //other.gameObject.GetComponent<ParticleSystem>().Play();
                    StartCoroutine(other.transform.Find("BoscoStick").gameObject.GetComponent<PlayerAction>().Stunned(2));
                    StartCoroutine(other.transform.Find("Player Camera").GetComponent<CameraShake>().Shake(true));
                    other.gameObject.GetComponent<PlayerAction>().BOOP();
                    takePokeDamageParticles.Play();
                    break;
                case PlayerAction.Action.SWING: // poke swinging player (SELF LOSE)
                    // OLD INTERACTION IN CASE WE WANT TO REVERT
                    //StartCoroutine(selfAction.Stunned(false)); // short stun
                    //TODO: Consider changing parryPS
                    other.gameObject.GetComponent<HasHealth>().changeHealth(
                        other.gameObject.GetComponent<HasHealth>().damageVals[0] + boscoStick.d_buff);
                    playParticles();
                    StartCoroutine(cameraShake.Shake(false)); // short camera shake
                    break;
                default: // IDLE, POKE_IN, POKE_OUT, POKE, SWING_IN, SWING_OUT are all same
                    // ACCESS DAMAGE VALUE IN HASHEALTH
                    other.gameObject.GetComponent<HasHealth>().changeHealth(
                        other.gameObject.GetComponent<HasHealth>().damageVals[0] + boscoStick.d_buff);
                    other.gameObject.GetComponent<PlayerAction>().BOOP();
                    takePokeDamageParticles.Play();
                    break;
            } // switch
        }
    }

    //TESTING
    private void poke_in()
    {
        GetComponentInParent<HasPlayerController>().DisableStick();
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.POKE_IN;
        GetComponentInParent<HasStamina>().OnPoke();
    }
    private void poke()
    {
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.POKE;
    }
    private void poke_out()
    {
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.POKE_OUT;
    }

    public void playParticles()
    {
        parryParticleSystem.Play();
    }
}
