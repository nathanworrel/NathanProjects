using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Attached to UI elements for pickups.
Serialized field lets us decide which string it activates off of.
*/


public class HasInventoryUI : MonoBehaviour
{
    Subscription<PowerUpEvent> powerup_event_subscription;
    public GameObject pickupUI;
    public Text tomatoText;
    private int num_tomatoes;

    [SerializeField]
    private int player;

    [SerializeField]
    private string powerUpName;


    // Start is called before the first frame update
    void Start()
    {
        powerup_event_subscription = EventBus.Subscribe<PowerUpEvent>(_OnPickup);
        tomatoText = tomatoText.GetComponent<Text>();
        num_tomatoes = 0;
        tomatoText.text = num_tomatoes.ToString();       
    }

    void _OnPickup(PowerUpEvent e)
    {
        if(player == e.player && powerUpName == e.powerup)
        {
            if(e.expired)
            {
                if(powerUpName == "tomato")
                {
                    num_tomatoes--;
                    if(num_tomatoes < 0)
                    {
                        num_tomatoes = 0;
                    }
                    tomatoText.text = num_tomatoes.ToString();
                }
                else
                {
                    pickupUI.SetActive(false);
                }
            }
            else
            {
                if(powerUpName == "tomato")
                {
                    num_tomatoes++;
                    tomatoText.text = num_tomatoes.ToString();
                }
                else
                {
                    pickupUI.SetActive(true);
                }
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(powerup_event_subscription);
    }
}

/*
PowerUp Types
Shield = "shield"
Throwable Object = "tomato"
DamagePickup = "damage"
God Powerup = "god"
*/