using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnRandomizer : MonoBehaviour
{
    /*
     * This scipt is ONLY called in PlayerManager.cs 
     * This script MUST be attached to the parent object of respawn locations
    */
    public Vector3 GetRandomRespawnLocation()
    {
        int randomNumber = Random.Range(0, transform.childCount);

        return transform.GetChild(randomNumber).position;
    }
}
