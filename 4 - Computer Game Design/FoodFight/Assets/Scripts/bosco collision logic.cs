using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boscocollisionlogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*  string neut = "neutral"
     *  string atk = "swinging"
     *  string poke = "poking"
     *  string block = "blocking"
     *  
     *  if swing PRESSED
     *      change state to atk
     *      do atk animation
     *      disable stick input for time
     *      after time, change state to neutral
     *  if poke PRESSED
     *      change state to poke
     *      do poke animation
     *      disable stick input for time
     *      after time, change state to neutral
     *  if block HELD
     *      change state to block WHILE HELD
     *      block animation WHILE HELD
     *      disable other stick inputs while blocking?
     *      !!OR!! if other stick input pressed, go to their function instead
     *      when released, change state to neutral
     *  on collision
     *      if other is not OTHER player (or stick?) (wall, floor, etc)
     *          "clank" animation for time and return to normal after (lower priority)
     *      if other is OTHER player (or stick?)
     *          if this state is atk
     *              if other is atk
     *                  Do something we haven't decided
     *              if otheris poke
     *                  OTHER player takes damage
     *              if other is block
     *                  I forget if discussed
     *              if other is neutral
     *                  other takes damage???
     *          if this state is poke
     *              if other is poke
     *                  Do something we haven't discussed
     *              if other is block
     *                  OTHER gets staggered
     *              if other is neutral
     *                  ???
     *          if this state is block
     *              ???
     *          
     *  
     *          
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
}
