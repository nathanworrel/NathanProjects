using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class PlayerAction : MonoBehaviour
{
    Dictionary<string, object> eventParams;

    public enum Action
    { 
        IDLE,
        SWING_IN,
        SWING,
        SWING_OUT,
        BLOCK_IN,
        BLOCK,
        POKE_IN,
        POKE,
        POKE_OUT,
        STUNNED,
    }

    public bool MAKE_PLAYERACTION_STATIC;
    public Action playerAction = Action.IDLE;
    private GameObject stick;
    private Animator anim;
    float timer;
    string swings = "";
    string pokes = "";
    string blocks = "";
    string stuns = "";
    string locations = "";

    // array to accesss for stun values
    private float[] stunTimes = { 0.95f, 1.15f, 1.5f };
    

    // Start is called before the first frame update
    void Start()
    {
        eventParams = new Dictionary<string, object>();
        timer = Time.time + 1;
        if (!MAKE_PLAYERACTION_STATIC)
            playerAction = Action.IDLE;
        stick = transform.Find("BoscoStick").gameObject;
        anim = stick.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (timer < Time.time)
        {
            locations += "{( " + transform.position + ")}";
            timer = Time.time + 1;
        }
    }

    /*public IEnumerator SetSwinging(float timeOn, float timeOf)
    {
        yield return new WaitForSecondsRealtime(timeOn);
        playerAction = Action.SWING;
        StartCoroutine(SetIdle(timeOf));
        // push event saying player swung
        swings += "{ (" + transform.position + ") - " + Time.time + "} ";
    }*/
    public void swingEvent()
    {
        // push event saying player swung
        swings += "{ (" + transform.position + ") - " + Time.time + "} ";
    }

    public void blockEvent()
    {
        // push event saying player blocked
        blocks += "{ (" + transform.position + ") - " + Time.time + "} ";
    }

    public void pokeEvent()
    {
        // push event saying player poked
        pokes += "{ (" + transform.position + ") - " + Time.time + "} ";
    }
    /*public IEnumerator SetPoking(float timeOn, float timeOf)
    {
        yield return new WaitForSeconds(timeOn);
        playerAction = Action.POKE;
        StartCoroutine(SetIdle(timeOf));
        // push event saying player poked
        pokes += "{ (" + transform.position + ") - " + Time.time + "} ";
    }
    public IEnumerator SetIdle(float timer)
    {
        yield return new WaitForSecondsRealtime(timer);
        playerAction = Action.IDLE;
    }
    */
    public IEnumerator Stunned(int stunVal)
    {
        Debug.Log("Stunned for" + stunVal);
        anim.SetBool("Stunned", true);
        anim.SetBool("Block", false);
        playerAction = Action.STUNNED;
        // set a playeraction stunned state
        yield return new WaitForSeconds(stunTimes[stunVal]);
        anim.SetBool("Stunned", false);
        playerAction = Action.IDLE;

        // push event saying player got stunned
        stuns += "{ (" + transform.position + ") - " + Time.time + "} ";
    }

    // OLD STUNNED FUNCTION IN CASE WE WANT TO REVERT
    /*public IEnumerator Stunned(bool longStun)
    {
        anim.SetTrigger("Stunned");
        anim.SetBool("Block", false);
        playerAction = Action.STUNNED;
        if (longStun)
        {
            yield return new WaitForSeconds(3f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        playerAction = Action.IDLE;
        // push event saying player got stunned
        stuns += "{ (" + transform.position + ") - " + Time.time + "} ";
    }*/

   /* public void SetBlock()
    {
        playerAction = Action.BLOCK;
        // push event saying player blocked
        blocks += "{ (" + transform.position + ") - " + Time.time + "} ";
    }*/

    public void SetIdle()
    {
        playerAction = Action.IDLE;
    }

    public void Knockback(int severity)
    {
        Debug.Log("KNOCKEDBACK");
    }

    public void BOOP()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 boopVec = new Vector3(0, 3f, 0);
        rb.AddForce(boopVec, ForceMode.Impulse);
    }

    // Internet said to include this to ensure analytics Vector3 gets pushed
    private void OnDestroy()
    {
        eventParams.Add("Swings", swings);
        eventParams.Add("Pokes", pokes);
        eventParams.Add("Blocks", blocks);
        eventParams.Add("Stuns", stuns);
        eventParams.Add("Locations", locations);
        AnalyticsService.Instance.CustomData("PlayerData", eventParams);
        AnalyticsService.Instance.Flush();
    }


}
