using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HasDeathText : MonoBehaviour
{
    public int player = 0;
    Subscription<DeathEvent> death_event_subscription;

    void Start()
    {
        death_event_subscription = EventBus.Subscribe<DeathEvent>(_OnDeathShow);
    }

    void _OnDeathShow(DeathEvent e){
        if(e.player == player){
            StartCoroutine(WaitForText(e.time));
        }
    }

    public IEnumerator WaitForText(float time)
    {
        GetComponent<Text>().enabled = true;

        yield return new WaitForSeconds(time);

        GetComponent<Text>().enabled = false;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(death_event_subscription);
    }
}
