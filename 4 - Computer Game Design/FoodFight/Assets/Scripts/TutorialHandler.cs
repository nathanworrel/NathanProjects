using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{    
    public GameObject inTutorial;
    public PlayerManager pm;
    PlayerAction selfAction;

    [SerializeField]
    private int player = 0;
    private string eventDescription = "";
    private bool canCont;
    Gamepad gamepad;
    public tutState state;

    private string welcome = "Welcome to Food Fight! Throughout this tutorial, press B (Xbox) or Circle (PS) to continue";
    private string intro1 = "This is a fighting game based on rock paper scissors";
    private string move = "Move with the left joystick and look around with the right joystick";
    private string jump = "Press A(Xbox) / X(Playstation) to jump";
    private string sprint = "Press into the left joystick while moving to toggle sprint";
    private string swing = "Press Right Trigger to swing";
    private string poke = "Press Right Shoulder to poke";
    private string block = "Press and hold Left Trigger to block";
    private string explain = "Each option is strong against another";
    private string explain2 = "Swing beats Poke. Poke beats Block. Block beats Swing.";
/*    private string explainBlock = "Block a swing to take no damage and ministun your opponent.";
    private string explainPoke = "Poke a blocking player to stun them for a long time";
    private string explainSwing = "Swing at a poking player to negate their attack and ministun them";*/
    private string stamina = "Each of these actions drains stamina, be careful not to run out";
    //private string dummies = "If you want to play around with these, there are test dummies here constantly swinging, poking, and blocking";
    private string powerUp = "At times, powerups will spawn around the arena, keep an eye out for them";
    private string tomato = "You can also pick up tomatoes, which can be thrown using Left Shoulder to do minor damage and disorient your foe";
    private string finish = "Go stand on the pressure plate whenever you're ready to play";

    public enum tutState
    {
        Welcome_,
        Intro1_,
        Move_,
        Sprint_,
        Swing_,
        Poke_,
        Block_,
        Explain_,
        Explain2_,
/*        ExplainBlock_,
        ExplainPoke_,
        ExplainSwing_,*/
        Stamina_,
        //Dummies_,
        Jump_,
        Tomato_,
        PowerUp_,
        Finish_,
    }


    // Start is called before the first frame update
    void Start()
    {
        canCont = false;
        inTutorial = GameObject.Find("TutorialHandler");
        selfAction = GetComponent<PlayerAction>();

        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>();
        if(pm.players.Count == 1)
        {
           player = 0;
        }
        else if(pm.players.Count == 2)
        {
            player = 1;
        }
        if(inTutorial)
        {
            StartCoroutine(TutorialStart());
        }
        // if it returns null, then we don't access this function
    }

    // This function checks if players do the correct input at the right time
    void Update()
    {
        gamepad = Gamepad.current;
    }
    
    public void onContinue(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canCont)
        {
            StartCoroutine(disableContinue());
            switch (state)
            {
                case tutState.Welcome_:
                    state = tutState.Intro1_;
                    newTutEvent(intro1);
                    break;
                case tutState.Intro1_:
                    state = tutState.Move_;
                    newTutEvent(move);
                    break;
                case tutState.Move_:
                    state = tutState.Jump_;
                    newTutEvent(jump);
                    break;
                case tutState.Jump_:
                    state = tutState.Sprint_;
                    newTutEvent(sprint);
                    break;
                case tutState.Sprint_:
                    state = tutState.Swing_;
                    newTutEvent(swing);
                    break;
                case tutState.Swing_:
                    state = tutState.Poke_;
                    newTutEvent(poke);
                    break;
                case tutState.Poke_:
                    state = tutState.Block_;
                    newTutEvent(block);
                    break;
                case tutState.Block_:
                    state = tutState.Explain_;
                    newTutEvent(explain);
                    break;
                case tutState.Explain_:
                    state = tutState.Explain2_;
                    newTutEvent(explain2);
                    break;
                case tutState.Explain2_:
                    state = tutState.Stamina_;
                    newTutEvent(stamina);
                    break;
                case tutState.Stamina_:
                    state = tutState.PowerUp_;
                    newTutEvent(powerUp);
                    break;
                case tutState.PowerUp_:
                    state = tutState.Tomato_;
                    newTutEvent(tomato);
                    break;
                case tutState.Tomato_:
                    state = tutState.Finish_;
                    newTutEvent(finish);
                    break;
                default:
                    break;
            }
        }
    }
    IEnumerator TutorialStart()
    {
        yield return new WaitForSeconds(1.0f);
        canCont = true;
        newTutEvent(welcome);
        state = tutState.Welcome_;
        yield return null;
    }
    IEnumerator disableContinue()
    {
        canCont = false;
        yield return new WaitForSeconds(0.2f);
        canCont = true;
    }
    
    public void OpenDoor()
    {
        eventDescription = "door";
        EventBus.Publish<TutorialEvent>(new TutorialEvent(player, eventDescription));
    }
    private void newTutEvent(string ev)
    {
        EventBus.Publish<TutorialEvent>(new TutorialEvent(player, ev));
    }
}


public class TutorialEvent
{
    public int player = 0;
    public string description = "Empty Data";
    public TutorialEvent(int _player, string _description) { player = _player; description = _description; }

    public override string ToString()
    {
        return "Player " + player + " has done " + description;
    }
}