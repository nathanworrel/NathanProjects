using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HasTutorialText : MonoBehaviour
{

    Subscription<TutorialEvent> tutorial_event_subscription;
    public Text tutorialText;

    public GameObject door1;
    public GameObject door2;

    [SerializeField]
    private int player;

    public string welcome;
    public string finish; 
    // Start is called before the first frame update
    void Start()
    {
        finish = "Go stand on the pressure plate whenever you're ready to play";
        welcome = "Welcome to Food Fight! Throughout this tutorial, press B (Xbox) or Circle (PS) to continue";
        tutorial_event_subscription = EventBus.Subscribe<TutorialEvent>(_OnTutorialEvent);
        tutorialText.enabled = false;
    }
    
    void _OnTutorialEvent(TutorialEvent e)
    {
        if(e.player == player)
        {
            if (e.description == welcome)
            {
                Debug.Log("text should get enabled");
                tutorialText.enabled = true;
            }
            tutorialText.text = e.description;
            if(e.description == finish)
            {
                if(e.player == 0)
                {
                    door1.SetActive(false);
                }
                else if (e.player == 1)
                {
                    door2.SetActive(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(tutorial_event_subscription);
    }
}
