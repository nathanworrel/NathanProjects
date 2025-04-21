using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider healthBar;

    Subscription<HealthEvent> subscription_health_event;
    [SerializeField] int health;
    [SerializeField] int player;
    [SerializeField] int maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        subscription_health_event = EventBus.Subscribe<HealthEvent>(_OnHealthChange);
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    // Update is called once per frame
    void Update()
    {
       healthBar.maxValue = maxHealth;
       healthBar.value = health;
    }

    void _OnHealthChange(HealthEvent e)
    {
        if(player == e.player)
        {
            if (player == e.player)
            {
                health = e.health;
                maxHealth = e.maxHealth;
            }
        }
    }
    private void OnDestroy()
    {
        EventBus.Unsubscribe(subscription_health_event);
    }
}
