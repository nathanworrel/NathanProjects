using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HasRedScreen : MonoBehaviour
{
    public int player;
    Image image;
    Subscription<DamageEvent> damage_event_subscription;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        damage_event_subscription = EventBus.Subscribe<DamageEvent>(_OnFade);
    }

    void _OnFade(DamageEvent e)
    {
        if(e.player == player){
            StartCoroutine(Fade(1,1,0));
        }
    }

    public IEnumerator Fade(float time, float start, float end){
        var tempColor = image.color;
        tempColor.a = start;
        image.color = tempColor;

        float inital_time = Time.time;
        float progress = (Time.time - inital_time) / time;

        while(progress < 1.0f){
            progress = (Time.time - inital_time) / time;

            float new_color = Mathf.Lerp(start, end, progress);

            tempColor.a = new_color;
            image.color = tempColor;

            yield return null;
        }
    }
}


public class DamageEvent
{
    public int player = 0;
    public DamageEvent(int _player) { player = _player; }

    public override string ToString()
    {
        return "Player " + player + " has been damaged";
    }
}