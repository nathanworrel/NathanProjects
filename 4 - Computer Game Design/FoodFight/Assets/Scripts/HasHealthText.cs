using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HasHealthText : MonoBehaviour
{
    Subscription<HealthEvent> health_event_subscription;
    private int health = 10;
    Text myText;
    private int playerNum;

    [SerializeField]
    private int player;
    

    void Start()
    {
        health_event_subscription = EventBus.Subscribe<HealthEvent>(_OnHealthChange);
        myText = GetComponent<Text>();
        playerNum = player + 1;
        myText.text = "Player " + playerNum + " Health = " + health;
    }

    void _OnHealthChange(HealthEvent e)
    {
        if(player == e.player)
        {
            health = e.health;
            myText.text = "Player " + playerNum + " Health = " + health;
        }
    }


    private void OnDestroy()
    {
        EventBus.Unsubscribe(health_event_subscription);
    }
}
