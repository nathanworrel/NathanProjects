using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HasFadePanel : MonoBehaviour
{
    Subscription<FadeEvent> fade_event_subscription;
    float time = 1f;
    public Image image;

    void Start()
    {
        image = GetComponent<Image>();
        fade_event_subscription = EventBus.Subscribe<FadeEvent>(_OnFade);
        StartCoroutine(Fade(2, 1, 0));
    }

    void _OnFade(FadeEvent e)
    {
        StartCoroutine(Fade(e.time, e.start, e.end));
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

    private void OnDestroy()
    {
        EventBus.Unsubscribe(fade_event_subscription);
    }
}

public class FadeEvent
{
    public float time = 0;
    public float start = 0;
    public float end = 0;
    public FadeEvent(float _time, float _start, float _end) { time = _time; start = _start; end = _end; }

    public override string ToString()
    {
        return "Fade for " + time + " from " + start + " to " + end;
    }
}