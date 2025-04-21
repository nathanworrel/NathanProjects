using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasGodPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            GameObject stick = other.transform.GetChild(1).gameObject;
            stick.GetComponent<BoscoStick>().DBuff();
            other.GetComponent<HasInventory>().CallEvent("damage", false);
            other.GetComponent<HasHealth>().MakeShield();
            other.GetComponent<HasInventory>().CallEvent("shield", false);
            Destroy(gameObject);
        }
    }
}
