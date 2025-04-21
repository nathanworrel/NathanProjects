using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
  This script is to set the player model / player skin.
  This changes the player model and the color of the hand holding the bosco stick.
*/

public class HasPlayerModel : MonoBehaviour
{
    public PlayerManager pm;
    public Renderer rend;

    public GameObject costume1;
    public Material material1;

    public GameObject costume2;
    public Material material2;

    public GameObject costume3;
    public Material material3;

    public GameObject hand;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        pm = FindObjectOfType<PlayerInputManager>().GetComponent<PlayerManager>();
        if(pm.players.Count == 1)
        {
            // set first costume active
            costume1.SetActive(true);
            costume2.SetActive(false);
            costume3.SetActive(false);

            // change the material for the hand
            hand.GetComponent<MeshRenderer>().material = material1;
            
            // turn off the renderer for the default look
            rend.enabled = false;
            // move hit box back
        }
        else if(pm.players.Count == 2)
        {
            // set second costume active
            costume2.SetActive(true);
            costume1.SetActive(false);
            costume3.SetActive(false);

            // change the material for the hand
            hand.GetComponent<MeshRenderer>().material = material2;
            
            // turn off the renderer for the default look
            rend.enabled = false;
            
        }
        else
        {
            // set third/error costume active
            costume1.SetActive(false);
            costume2.SetActive(false);
            costume3.SetActive(true);

            // change the material for the hand
            hand.GetComponent<MeshRenderer>().material = material3;

            // turn off the renderer for the default look
            rend.enabled = false;
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
