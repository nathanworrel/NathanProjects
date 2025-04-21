using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HasInventory : MonoBehaviour
{

    public PlayerManager pm;
    private int player = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>();
        if(pm.players.Count == 1)
        {
            player = 0;
        }
        else
        {
            player = 1;
        }
        EventBus.Publish<PowerUpEvent>(new PowerUpEvent(player, true, "shield")); //initial publish
        EventBus.Publish<PowerUpEvent>(new PowerUpEvent(player, true, "damage")); //initial publish
    }

    // Publishes event when function is called.
    public void CallEvent(string powerupType, bool expired)
    {
        EventBus.Publish<PowerUpEvent>(new PowerUpEvent(player, expired, powerupType));
    }
}

public class PowerUpEvent
{
    public int player = 0;
    public bool expired = true;
    public string powerup = "";
    public PowerUpEvent(int _player, bool _expired, string _powerup) { player = _player; expired = _expired; powerup = _powerup; }

    public override string ToString()
    {
        return "Player " + player + " has received powerup: " + powerup;
    }
}

/*
PowerUp Types
Shield = "shield"
Throwable Object = "tomato"
DamagePickup = "damage"
God Powerup = "god"
*/