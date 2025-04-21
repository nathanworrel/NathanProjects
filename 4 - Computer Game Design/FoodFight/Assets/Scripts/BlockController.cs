using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    //public GameObject self;
    //PlayerAction selfAction;

    // Start is called before the first frame update
    void Start()
    {
    //    selfAction = self.GetComponent<PlayerAction>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // BLOCK IS NEVER AN AGGRESSOR, THEREFORE IT WONT INITIATE ANY COLLISIONS

    private void block_in()
    {
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.BLOCK_IN;
        GetComponentInParent<HasPlayerController>().DisableStick();
    }

    private void block()
    {
        GetComponentInParent<PlayerAction>().playerAction = PlayerAction.Action.BLOCK;
    }
}
