using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class HasStamina : MonoBehaviour
{
    private float MStamina = 50.0f;
    [SerializeField] float CStamina;
    private int player = 0;

    [SerializeField] bool isSprinting = false;

    PlayerManager pm;
    PlayerAction selfAction;

    [SerializeField] float StaminaRegenTimer = 0.0f;
    [SerializeField] float StaminaIncreasePerFrame;
    [SerializeField] float StaminaTimeToRegen;

    [SerializeField] float WhileSprintingDecrease;
    [SerializeField] float OnBlockDecrease;
    [SerializeField] float WhileBlockingDecrease;
    [SerializeField] float OnSwingDecrease;
    [SerializeField] float OnPokeDecrease;

    private float blockTime;
    [SerializeField] float swingTime;
    [SerializeField] float pokeTime;

    private float SwingDuration;
    private float PokeDuration;
    private float BlockDuration;

    private bool swingCalc;
    private bool pokeCalc;

    private int frameCounter;

    private float totalOBD; // TOTAL ON BLOCK DECREASE, USED TO CALCULATE OTHER DECREASES
    Animator animator;

    public GameObject sprintParticles;

    public AudioClip sound_clip_swing;
    public AudioClip sound_clip_poke;
    Vector3 sound_position;

    Rumbler rumblerThing;

    // Start is called before the first frame update
    void Start()
    {
        swingCalc = false;
        pokeCalc = false;
        SwingDuration = GetComponent<HasPlayerController>().getSwingVar();
        PokeDuration = GetComponent<HasPlayerController>().getPokeVar();
        BlockDuration = GetComponent<HasPlayerController>().getBlockVar();
        animator = transform.Find("BoscoStick").GetComponent<Animator>();
        StaminaTimeToRegen = 0.625f;
        WhileSprintingDecrease = 7.0f;
        OnBlockDecrease = 11.125f;
        totalOBD = OnBlockDecrease * BlockDuration;
        OnSwingDecrease = totalOBD * 2.75f;
        OnPokeDecrease = totalOBD * 1.25f;
        /*OnSwingDecrease = totalOBD / SwingDuration * 2.75f;
        OnPokeDecrease = totalOBD / PokeDuration * 1.25f;*/
        StaminaIncreasePerFrame = 14.5f;
        WhileBlockingDecrease = 5.5f;
        CStamina = MStamina;
        sound_position = new Vector3(0, 0, 0);
        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>();
        if (pm.players.Count == 1)
        {
            player = 0;
        }
        else
        {
            player = 1;
        }
        EventBus.Publish<StaminaEvent>(new StaminaEvent(player, CStamina, MStamina));

        rumblerThing = GetComponent<Rumbler>();
    }

    // Update is called once per frame
    void Update()
    {
        frameCounter++;
        if (frameCounter == 60)
        {
            frameCounter = 0;
        }

        isSprinting = GetComponent<HasPlayerController>().sprintCheck();

        if (isSprinting)
        {
            _ChangeStamina(WhileSprintingDecrease);
            StaminaRegenTimer = 0;

            // spawn sprint particles vv
            if (frameCounter % 10 == 0)
            {
                GameObject particleInstance = GameObject.Instantiate(sprintParticles);
                Vector3 playerFeet = this.transform.position;
                playerFeet.y -= 1;
                particleInstance.transform.position = playerFeet;
            }
        }
        else if (animator.GetBool("Block") == true)
        {
            if(GetComponent<PlayerAction>().playerAction == PlayerAction.Action.BLOCK_IN)
            {
                _ChangeStamina(OnBlockDecrease);
            }
            if(GetComponent<PlayerAction>().playerAction == PlayerAction.Action.BLOCK)
            {
                _ChangeStamina(WhileBlockingDecrease);
            }
            StaminaRegenTimer = 0;
        }
        /*else if (swingCalc == true)
        {
            if (Time.time - swingTime < SwingDuration)
            {
                _ChangeStamina(OnSwingDecrease);
            }
            else
            {
                swingCalc = false;
            }
            StaminaRegenTimer = 0;
        } 
        else if(pokeCalc == true)
        {
            if (Time.time - pokeTime < PokeDuration)
            {
                _ChangeStamina(OnPokeDecrease);
            }
            else
            {
                pokeCalc = false;
            }
            StaminaRegenTimer = 0;
        }*/
        else if (CStamina < MStamina && 
            (GetComponent<PlayerAction>().playerAction == PlayerAction.Action.IDLE || GetComponent<PlayerAction>().playerAction == PlayerAction.Action.STUNNED))
        {
            //TODO: Decide if we want to regen stamina while stunned
            if (StaminaRegenTimer >= StaminaTimeToRegen)
                _ChangeStamina(-StaminaIncreasePerFrame);
            else
                StaminaRegenTimer += Time.deltaTime;
        }
        EventBus.Publish<StaminaEvent>(new StaminaEvent(player, CStamina, MStamina));
    }

    //OLD VERSION
/*    public void OnBlock()
    {
        blockTime = Time.time;
    }*/
    /*public void OnSwing()
    {
        swingTime = Time.time;
        swingCalc = true;
    }*/
    public void OnSwing()
    {
        _InstantChangeStamina(OnSwingDecrease);
        StaminaRegenTimer = 0;
        StartCoroutine(PlayAudioClipDelayed(sound_clip_swing, 0.8f));
    }
    /*public void OnPoke()
    {
        pokeTime = Time.time;
        pokeCalc = true;
    }*/
    public void OnPoke()
    {
        _InstantChangeStamina(OnPokeDecrease);
        StaminaRegenTimer = 0;
        StartCoroutine(PlayAudioClipDelayed(sound_clip_poke, 0.8f));
    }
    public float getStamina()
    {
        return CStamina;
    }
    public bool GetSprinting()
    {
        return isSprinting;
    }
    private void _ChangeStamina(float value)
    {
        CStamina = Mathf.Clamp(CStamina - (value * Time.deltaTime), 0.0f, MStamina);
        if(CStamina == 0){
            rumblerThing.RumblePulse(0.2f, 0.2f, 0.05f, 0.5f);
        }
    }
    private void _InstantChangeStamina(float value)
    {
        CStamina = Mathf.Clamp(CStamina - value, 0.0f, MStamina);
        if(CStamina == 0){
            rumblerThing.RumblePulse(0.2f, 0.2f, 0.05f, 0.5f);
        }
    }
    public void resetStaminaTimer()
    {
        StaminaRegenTimer = 0.0f;
    }

    public IEnumerator PlayAudioClipDelayed(AudioClip sound_clip, float time_wait)
    {
        yield return new WaitForSeconds(time_wait);
        AudioSource.PlayClipAtPoint(sound_clip, sound_position);
        yield return null;
    }
}

public class StaminaEvent
{
    public int player = 0;
    public float stamina = 0;
    public float maxStamina = 0;
    public StaminaEvent(int _player, float _stamina, float _maxStamina) { player = _player; stamina = _stamina; maxStamina = _maxStamina; }

    public override string ToString()
    {
        return "Player " + player + " has " + stamina + " out of " 
            + maxStamina + " stamina";
    }
}
