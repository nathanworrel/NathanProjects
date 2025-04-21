using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StaminaBar : MonoBehaviour
{
    public Slider staminaBar;

    Subscription<StaminaEvent> subscription_stamina_event;
    [SerializeField] float stamina;
    [SerializeField] int player;
    [SerializeField] float maxStamina;
    // Start is called before the first frame update
    void Start()
    {
        subscription_stamina_event = EventBus.Subscribe<StaminaEvent>(_OnStaminaChange);
        staminaBar = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.maxValue = maxStamina;
        staminaBar.value = stamina;
    }

    void _OnStaminaChange(StaminaEvent e)
    {
        if(player == e.player)
        {
            stamina = e.stamina;
            maxStamina = e.maxStamina;
        }
    }
    private void OnDestroy()
    {
        EventBus.Unsubscribe(subscription_stamina_event);
    }
}
