using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeIcons : MonoBehaviour
{
    Subscription<DeathEvent> subscription_death_event;
    [SerializeField] int player;
    [SerializeField] int maxLives;
    private int currentLives;

    public GameObject[] heartIcons;


    // Start is called before the first frame update
    void Start()
    {
        subscription_death_event = EventBus.Subscribe<DeathEvent>(_OnPlayerDeath);
        currentLives = maxLives;
    }

    void _OnPlayerDeath(DeathEvent e)
    {
        if(player == e.player)
        {
            if(currentLives > 0)
            {
                heartIcons[currentLives - 1].SetActive(false);
                currentLives--;
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(subscription_death_event);
    }
}
