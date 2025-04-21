using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingController : MonoBehaviour
{
    public GameObject self;
    public ParticleSystem takeSwingDamageParticles;
    public GameObject parryParticles;

    ParticleSystem parryParticleSystem;


    ParticleController particleController;


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

        particleController = GetComponent<ParticleController>();
    }

    private void OnTriggerEnter(Collider other){
        collided(other);
    }
    
    private void OnTriggerStay(Collider other) {
        collided(other);
    }

    //TODO: Fix Camera Shake
    private void collided(Collider other)
    {
        // self is swinging && hit a player && not yourself
        if (selfAction.playerAction == PlayerAction.Action.SWING
             && other.gameObject.CompareTag("Player")
             && other.gameObject != self
             && timer < Time.time)
        {
            timer = Time.time + spaced_time;
            Debug.Log("Trigger collision detected");
            PlayerAction.Action opponentAction = other.gameObject.GetComponent<PlayerAction>().playerAction;
            Debug.Log(selfAction.playerAction + " " + opponentAction);
            switch (opponentAction)
            {
                case PlayerAction.Action.BLOCK: // swing blocking player (SELF LOSE)
                    // DO NOT CHANGEE VALUES IN .Stunned. 
                    StartCoroutine(selfAction.Stunned(0)); // low stun
                    parryParticleSystem.Play();
                    StartCoroutine(cameraShake.Shake(true)); // long camera shake
                    break;
                case PlayerAction.Action.BLOCK_IN:
                    StartCoroutine(selfAction.Stunned(1)); // mid stun
                    // TODO: Different particle system for parry (collision on block_in) than for Block
                    parryParticleSystem.Play();
                    StartCoroutine(cameraShake.Shake(true));
                    break;
                case PlayerAction.Action.POKE: // swing poking player (SELF WIN)
                    // high dmg
                    dealDamage(other, 2);
                    takeSwingDamageParticles.Play();
                    StartCoroutine(other.gameObject.GetComponentInChildren<CameraShake>().Shake(false)); // short camera shake
                    // self 
                    break;
                case PlayerAction.Action.POKE_IN:
                    //Same as POKE
                    dealDamage(other, 2);
                    takeSwingDamageParticles.Play();
                    StartCoroutine(other.gameObject.GetComponentInChildren<CameraShake>().Shake(false)); // short camera shake
                    // self 
                    break;
                case PlayerAction.Action.POKE_OUT:
                    dealDamage(other, 2);
                    takeSwingDamageParticles.Play();
                    StartCoroutine(other.gameObject.GetComponentInChildren<CameraShake>().Shake(false)); // short camera shake
                    break;
                default: // IDLE, SWING,STUNNED, SWING_IN, SWING_OUT, POKE_OUT all do same thing
                    //NOTE: Make OTHER TAKE MID DAMAGE, that way if swing itneracts swing its same as default
                    dealDamage(other, 1);
                    takeSwingDamageParticles.Play();
                    Debug.Log("Swinging on opponent test");
                    break;
            } // switch
        }
    }
    void dealDamage(Collider col, int dmg) // takes collider, always accesses damageVals[1] because swing will always do that value of damage
    {
        col.gameObject.GetComponent<HasHealth>().changeHealth(
            col.gameObject.GetComponent<HasHealth>().damageVals[dmg] + boscoStick.d_buff);
    }

    private void swing_in()
    {
        GetComponentInParent<HasPlayerController>().DisableStick();
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.SWING_IN;
        GetComponentInParent<HasStamina>().OnSwing();
    }
    private void swing()
    {
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.SWING;
    }
    private void swing_out()
    {
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.SWING_OUT;
        GetComponentInParent<HasStamina>().resetStaminaTimer();
    }

}
