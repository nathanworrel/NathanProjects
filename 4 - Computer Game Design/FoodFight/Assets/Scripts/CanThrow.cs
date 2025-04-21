using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanThrow : MonoBehaviour
{
    Subscription<PauseEvent> pause_event_subscription;

    public PlayerManager pm;

    private int player = 0;
    private bool paused = false;

    public GameObject projectile;
    public float launchVelocity = 100f;
    private int num_balls;

    // Start is called before the first frame update
    void Start()
    {
        pause_event_subscription = EventBus.Subscribe<PauseEvent>(_OnPauseChange);

        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>();
        if(pm.players.Count == 1)
        {
            player = 0;
        }
        else
        {
            player = 1;
        }
        num_balls = 0;
    }

    // Update is called once per frame
    public void ThrowObject()
    {
        if(!paused && num_balls > 0)
        {
            // spawns the ball in the direction that the player is facing (and doesn't collide with the player collider)
            GameObject ball = Instantiate(projectile, transform.position + (transform.forward * 2), transform.rotation);

            // launches the ball
            ball.GetComponent<Rigidbody>().velocity= (transform.forward * launchVelocity);
            num_balls --;
            GetComponent<HasInventory>().CallEvent("tomato", true);
        }
    }

    public void AddBalls(int i){
        num_balls ++;
        GetComponent<HasInventory>().CallEvent("tomato", false);
    }

    void _OnPauseChange(PauseEvent e)
    {
        paused = e.paused;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(pause_event_subscription);
    }
}
