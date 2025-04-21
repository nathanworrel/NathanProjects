using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasBallPickup : MonoBehaviour
{

    public AudioClip sound_clip;
    Vector3 sound_position;
    private int change_amount = 1;

    void Start()
    {
        sound_position = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            AudioSource.PlayClipAtPoint(sound_clip, sound_position);
            other.GetComponent<CanThrow>().AddBalls(change_amount);
            Destroy(gameObject);
            Debug.Log("Balled");
        }
    }
}
