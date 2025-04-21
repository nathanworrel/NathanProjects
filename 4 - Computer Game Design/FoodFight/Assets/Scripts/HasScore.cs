using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HasScore : MonoBehaviour
{
    Subscription<HealthEvent> health_event_subscription;
    private int score;
    private int player = 0;

    public PlayerManager pm;

    // Start is called before the first frame update
    void Start()
    {
        health_event_subscription = EventBus.Subscribe<HealthEvent>(_OnHealthChange);
        score = 3;

        // This code is duplicated in HasHealth as well
        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>(); 
        if(pm.players.Count == 1)
        {
            player = 0;
        }
        else
        {
            player = 1;
        }
        // Duplicated code ends here

        EventBus.Publish<ScoreEvent>(new ScoreEvent(player, score));
    }

    void _OnHealthChange(HealthEvent e)
    {
        Debug.Log("Health changed to 0" + e.health);
        if(player == e.player){
            if(e.health <= 0)
            {
                score--;
                EventBus.Publish<ScoreEvent>(new ScoreEvent(player, score));
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(health_event_subscription);
    }
}

public class ScoreEvent
{
    public int player = 0;
    public int score = 0;
    public ScoreEvent(int _player, int _score) { player = _player; score = _score; }

    public override string ToString()
    {
        return "Player " + player + " has " + score + " score";
    }
}
