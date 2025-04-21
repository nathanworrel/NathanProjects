using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatIconController : MonoBehaviour
{
    /*
     This script will be called in BOTH PlayerAction.cs AND HasPlayerController.cs
        PlayerAction.cs will handle SWING, POKE, and STUNNED icon instances
        HasPlayerController.cs will handle BLOCK icon instances
     Enable and disable rock/paper/scissors icons above players' heads
     TODO: add particles circling player to show stunned state
     */
    public GameObject blockIcon; // rock
    public GameObject pokeIcon; // paper
    public GameObject swingIcon; // scissors

    IconVisibilityController blockVisibility;
    IconVisibilityController pokeVisibility;
    IconVisibilityController swingVisibility;

    private void Start()
    {
        blockVisibility = blockIcon.GetComponent<IconVisibilityController>();
        pokeVisibility = pokeIcon.GetComponent<IconVisibilityController>();
        swingVisibility = swingIcon.GetComponent<IconVisibilityController>();
    }

    public IEnumerator EnableCombatIcon(float timer, PlayerAction.Action playerAction)
    {
        Debug.Log("ICON TRIGGER");
        switch (playerAction)
        {
            case PlayerAction.Action.BLOCK:
                blockVisibility.EnableIcon();
                break;
            case PlayerAction.Action.SWING:
                swingVisibility.EnableIcon();
                yield return new WaitForSeconds(timer);
                swingVisibility.DisableIcon();
                break;
            case PlayerAction.Action.POKE:
                pokeVisibility.EnableIcon();
                yield return new WaitForSeconds(timer);
                pokeVisibility.DisableIcon();
                break;
            case PlayerAction.Action.STUNNED:
                break;
        } // switch
    }

    public void DisableBlockIcon()
    {
        blockVisibility.DisableIcon();
    }

    public void DisableAllIcons()
    {
        blockVisibility.DisableIcon();
        swingVisibility.DisableIcon();
        pokeVisibility.DisableIcon();
    }
}
