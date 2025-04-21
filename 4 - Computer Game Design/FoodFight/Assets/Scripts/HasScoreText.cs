using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HasScoreText : MonoBehaviour
{
    Subscription<ScoreEvent> score_event_subscription;
    private int score = 3;
    Text myText;
    private int playerNum;

    [SerializeField]
    private int player;

    // Start is called before the first frame update
    void Start()
    {
        score_event_subscription = EventBus.Subscribe<ScoreEvent>(_OnScoreChange);
        myText = GetComponent<Text>();
        playerNum = player + 1;
        myText.text = "Player " + playerNum + " Lives = " + score;
    }

    void _OnScoreChange(ScoreEvent e)
    {
        if(player == e.player)
        {
            score = e.score;
            myText.text = "Player " + playerNum + " Lives = " + score;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(score_event_subscription);
    }
}
