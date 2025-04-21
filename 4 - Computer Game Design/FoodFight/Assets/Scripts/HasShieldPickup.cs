using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasShieldPickup : MonoBehaviour
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
            other.GetComponent<HasHealth>().MakeShield();
            other.GetComponent<HasInventory>().CallEvent("shield", false);
            Destroy(gameObject);
            Debug.Log("Shield");
        }
    }
}
