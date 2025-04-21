using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasDamagePickup : MonoBehaviour
{

    public AudioClip sound_clip;
    Vector3 sound_position;

    void Start()
    {
        sound_position = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            AudioSource.PlayClipAtPoint(sound_clip, sound_position);
            GameObject stick = other.transform.GetChild(1).gameObject;
            stick.GetComponent<BoscoStick>().DBuff();
            other.GetComponent<HasInventory>().CallEvent("damage", false);
            Destroy(gameObject);
        }
    }
}
