using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasPickup : MonoBehaviour
{
    public HasSpawner spawner;
    
    private void OnDestroy() {
        spawner.destroyed();
    }
}
