using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HasPauseMenu : MonoBehaviour
{

    Subscription<PauseEvent> pause_event_subscription;
    CanPauseGame canPause;
    public GameObject pauseMenu;

    void Start()
    {
        pause_event_subscription = EventBus.Subscribe<PauseEvent>(_OnPauseChange);
        canPause = GetComponent<CanPauseGame>();
        pauseMenu.SetActive(false);
    }

    public void PlayGame()
    {
        canPause.PauseGame();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    void _OnPauseChange(PauseEvent e)
    {
        if(e.paused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(pause_event_subscription);
    }
}
