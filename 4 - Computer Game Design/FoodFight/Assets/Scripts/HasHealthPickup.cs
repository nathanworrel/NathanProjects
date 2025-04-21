using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHealthPickup : MonoBehaviour
{
    private int change_amount = 4;
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            other.GetComponent<HasHealth>().changeHealth(change_amount);
            Destroy(gameObject);
            Debug.Log("healed");
        }
    }
}
