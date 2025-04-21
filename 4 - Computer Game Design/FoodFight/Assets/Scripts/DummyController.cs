using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    public enum Type
    {
        SWING,
        POKE,
        BLOCK,
    }
    public Type dType;

    private float interval = 2.0f;
    private float countTime;

    // Start is called before the first frame update
    void Start()
    {
        countTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        countTime += Time.deltaTime;
        if (dType != Type.BLOCK)
        {
            if (countTime > interval)
            {
                countTime = 0f;
                //shoot();
            }
        }
        else
        {

        }
    }

    private void act()
    {
        if (dType == Type.SWING)
        {
            Debug.Log("Dummy Swing");
        }
        else if(dType == Type.POKE)
        {
            Debug.Log("Dummy Poke");
        }
    }
}
