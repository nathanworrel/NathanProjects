using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoscoStick : MonoBehaviour
{

    public int d_buff = 0;

    public void OnSwing(InputAction.CallbackContext ctx)
    {
        //StartCoroutine(Swing());
        Debug.Log("Test Swing");
    }

    public void OnBlock(InputAction.CallbackContext ctx)
    {
        Debug.Log("Test block");
    }

    public void OnPoke(InputAction.CallbackContext ctx)
    {
        // StartCoroutine(Poke());
        Debug.Log("Test Poke");
    }

    IEnumerator Swing()
    {
        //yield return new WaitForSeconds(1f);
        yield return null;
    }

    IEnumerator Poke()
    {
        //yield return new WaitForSeconds(1f);
        yield return null;
    }

    public void DBuff(){
        StartCoroutine(damageBuff());
    }

    public IEnumerator damageBuff(){
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        d_buff = -1;

        yield return new WaitForSeconds(15f);

        GetComponentInParent<HasInventory>().CallEvent("damage", true);
        d_buff = 0;
        GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
    }

    private void enableStick()
    {
        GetComponentInParent<HasPlayerController>().EnableStick();
    }

    private void idleState() // This should not be run every time idle is cycled. This is awful. Fix eventually hopefully
    {
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.IDLE;
        // This catches for you to be able to use your stick if you get stunned out of another animation
        // CHANGE THIS WHEN WE IMPLEMENT A STUNNED ANIMATION STATE
        GetComponentInParent<HasPlayerController>().EnableStick(); 
                        
    }

}
