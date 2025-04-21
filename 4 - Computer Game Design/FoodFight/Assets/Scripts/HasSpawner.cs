using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasSpawner : MonoBehaviour
{
    public GameObject item;
    public float time_between_spawns = 30;
    float timer;
    bool spawned;
    GameObject hold;

    void Start()
    {
        timer = Time.time;
        spawned = false;
        if(item == null){
            Destroy(this);
        }
    }

    void Update()
    {
        if(!spawned && timer + time_between_spawns < Time.time){
            hold = Instantiate(item, transform.position+new Vector3(0,2,0), transform.rotation);
            hold.GetComponent<HasPickup>().spawner = this;
            spawned = true;
        }
    }

    public void destroyed(){
        hold = null;
        spawned = false;
        timer = Time.time;
    }
}
