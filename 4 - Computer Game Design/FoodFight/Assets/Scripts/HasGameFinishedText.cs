using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HasGameFinishedText : MonoBehaviour
{
    Subscription<ScoreEvent> score_event_subscription;
    Text gameFinishedText;

    // Start is called before the first frame update
    void Start()
    {
        score_event_subscription = EventBus.Subscribe<ScoreEvent>(_OnScoreUpdate);
        gameFinishedText = GetComponent<Text>();
        gameFinishedText.enabled = false;
    }

    void _OnScoreUpdate(ScoreEvent e)
    {
        if(e.score <= 0)
        {
            if(e.player == 0)
            {
                gameFinishedText.text = "Player 2 Wins";
            }
            else
            {
                gameFinishedText.text = "Player 1 Wins";
            }

            // enable text to show who won
            gameFinishedText.enabled = true;

            // Return to title screen after 3 seconds
            StartCoroutine(ReturnToTitle());
        }
    }

    public IEnumerator ReturnToTitle()
    {
        yield return new WaitForSeconds(1.0f);
        EventBus.Publish<FadeEvent>(new FadeEvent(2, 0, 1));
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("TitleScreen");
        yield return null;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(score_event_subscription);
    }
}
