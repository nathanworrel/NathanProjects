using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Analytics;

public class HasHealth : MonoBehaviour
{
    private int THealth = 10;
    private int CHealth;
    private int player = 0;
    public bool immune = false;
    private float time_shield = 15;
    private int shield_hit = 0;
    public HasShield shield;
    public HasShield face;

    public AudioClip sound_clip;
    public AudioClip sound_clip_health_gained;
    Vector3 sound_position;
    public PlayerManager pm;

    Rumbler rumblerThing;

    public int[] damageVals;
    private void Awake()
    {
        damageVals = new int[3];
    }
    void Start()
    {
        damageVals[0] = -1;
        damageVals[1] = -3;
        damageVals[2] = -4;
        CHealth = THealth;
        sound_position = new Vector3(0, 0, 0);
        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>();
        rumblerThing = GetComponent<Rumbler>();
        if(pm.players.Count == 1)
        {
            player = 0;
        }
        else
        {
            player = 1;
        }
        EventBus.Publish<HealthEvent>(new HealthEvent(player, CHealth, THealth));
    }

    public void changeHealth(int i){
        if(i < 0 && immune){
            shield_hit --;
            return;
        }
        Debug.Log("Old " + CHealth);
        CHealth += i;
        Debug.Log("New " + CHealth);
        Debug.Log("Health changed by" + i);
        // Play "oof" sound if int i is negative
        if(i < 0)
        {
            EventBus.Publish<DamageEvent>(new DamageEvent(player));
            AudioSource.PlayClipAtPoint(sound_clip, sound_position);
            rumblerThing.RumbleConstant(0.5f,0.5f,0.5f);
        }
        else if(i > 0) // play refreshing sound if health is gained.
        {
            AudioSource.PlayClipAtPoint(sound_clip_health_gained, sound_position);
        }

        if(CHealth <= 0){
            Debug.Log("Died");
            playerDeath();
            StartCoroutine(TemporaryInvincible(3.0f));
        }
        else{
            StartCoroutine(TemporaryInvincible(0.2f));
        }
        if(CHealth > THealth){
            CHealth = THealth;
        }
        EventBus.Publish<HealthEvent>(new HealthEvent(player, CHealth, THealth));
    }

    void playerDeath()
    {
        EventBus.Publish<HealthEvent>(new HealthEvent(player, CHealth, THealth));
        EventBus.Publish<DeathEvent>(new DeathEvent(player, 3.0f));
        // Send player to death area, then respawn
        StartCoroutine(pm.RespawnPlayer(player));
        StartCoroutine(magic());
        // Reset to full Health
        CHealth = THealth;
        // Publish Event at full hp
        EventBus.Publish<HealthEvent>(new HealthEvent(player, CHealth, THealth));
        // push event saying player died

        GetComponent<HasInventory>().CallEvent("damage", true);
        GetComponent<HasInventory>().CallEvent("shield", true);
    }
    IEnumerator magic()
    {
        if (player == 0)
        {
            transform.Find("Player Camera").GetComponent<Camera>().cullingMask |= 1 << 8;
            yield return new WaitForSeconds(3);
            transform.Find("Player Camera").GetComponent<Camera>().cullingMask &= ~(1 << 8);
        }
        else
        {
            transform.Find("Player Camera").GetComponent<Camera>().cullingMask |= 1 << 9;
            yield return new WaitForSeconds(3);
            transform.Find("Player Camera").GetComponent<Camera>().cullingMask &= ~(1 << 9);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= -10.0f) 
        {
            changeHealth(-CHealth);
            // push event saying player fell off map
        }
    }

    public IEnumerator TemporaryInvincible(float time)
    {
        immune = true;

        yield return new WaitForSeconds(time);

        immune = false;
    }

    public void MakeShield(){
        StartCoroutine(Shield());
    }

    private IEnumerator Shield(){
        shield_hit = 3;
        float timer = Time.time;
        immune = true;

        shield.Render(true);
        face.Render(true);

        while(shield_hit > 0 && timer + time_shield > Time.time){

            if(timer + (time_shield/3) < Time.time && shield_hit == 3){
                shield_hit = 2;
            }else if(timer +(2*time_shield/3) < Time.time && shield_hit == 2){
                shield_hit = 1;
            }
            shield.SetState(shield_hit);
            face.SetState(shield_hit);
            yield return null;
        }

        shield.Render(false);
        GetComponent<HasInventory>().CallEvent("shield", true);
        face.Render(false);

        immune = false;
    }

    // Internet said to include this to ensure analytics data gets pushed
    private void OnDestroy()
    {
        Analytics.FlushEvents();
    }
}

public class HealthEvent
{
    public int player = 0;
    public int health = 0;
    public int maxHealth = 0;
    public HealthEvent(int _player, int _health, int _maxHealth) { player = _player; health = _health; maxHealth = _maxHealth; }

    public override string ToString()
    {
        return "Player " + player + " has " + health + " out of " +
            maxHealth + " health";
    }
}

public class DeathEvent
{
    public int player = 0;
    public float time = 0;
    public DeathEvent(int _player, float _time) { player = _player; time = _time; }

    public override string ToString()
    {
        return "Player " + player + " has died";
    }
}