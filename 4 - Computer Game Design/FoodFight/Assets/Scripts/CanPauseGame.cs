using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanPauseGame : MonoBehaviour
{
    private bool paused;
    private int player = 0;

    public PlayerManager pm;

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>();
        if(pm.players.Count == 1)
        {
            player = 0;
        }
        else
        {
            player = 1;
        }
        EventBus.Publish<PauseEvent>(new PauseEvent(player, paused));
    }

    public void PauseGame()
    {
        //Change to pause or unpaused and publish the event
        paused = !paused;
        EventBus.Publish<PauseEvent>(new PauseEvent(player, paused));
    }
}

public class PauseEvent
{
    public int player = 0;
    public bool paused = false;
    public PauseEvent(int _player, bool _paused) { player = _player; paused = _paused; }

    public override string ToString()
    {
        return "Player " + player + " has paused.";
    }
}