using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasSpin : MonoBehaviour
{
    int speedX = 10;
    int speedY = 25;
    int speedZ = 50;
    void Update ()
    {
        transform.Rotate (speedX*Time.deltaTime,speedY*Time.deltaTime,speedZ*Time.deltaTime);
    }
}
