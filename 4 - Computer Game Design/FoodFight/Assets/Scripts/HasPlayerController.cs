using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HasPlayerController : MonoBehaviour
{
    public float cull;
    public float speed = 5;
    public float sprintSpeed = 7;
    public float jump_height = 5;
    private Vector2 movementInput;
    private Rigidbody rb;
    public Animator animator;
    //private Camera playerCam;
    private Vector3 gtl; // ground detector location
    private float gtr; // ground detector radius
    [SerializeField] float rayDist;
    [SerializeField] bool canJump = false;
    [SerializeField] bool isMoving = false;
    bool canMove = true;
    [SerializeField] RaycastHit hit;

    /*    private float walkField;
        private float sprintField;
        private float blockField;
        private float attackField;*/


    PlayerInput playerInput;
    PlayerAction selfAction;
    CanPauseGame canPause;
    CanThrow canThrow;

    [SerializeField] bool canStick;
    [SerializeField] float delayStick;

    Subscription<PauseEvent> pause_event_subscription;

    [SerializeField] float CStamina;
    [SerializeField] bool isSprinting = false;

    private float setSwingVar;
    private float setPokeVar;
    private float setBlockVar;
    private float timeOnPoke;
    private float timeOfPoke;
    private float timeOnSwing;
    private float timeOfSwing;

    // SPRINT FOV VARIABLES
    Camera playerCamera;
    [SerializeField]
    private float walkFOV;
    [SerializeField]
    private float sprintFOV;
    private float sprintFOVDuration;
    private float sprintStartTime;
    private bool changeSprintFOV = false;

    private void Awake()
    {
        delayStick = 0.6f;
        setSwingVar = 1.6579f;
        setPokeVar = 1.8078f;
        setBlockVar = 0.36488f;
        timeOnPoke = 0.68f;
        timeOnSwing = 0.87f;
        timeOfPoke = 0.249f;
        timeOfSwing = 0.2798f;
    }
    private void Start()
    {
        // These are currently unused. May be helpful for juice later
        /*playerCam = transform.Find("Player Camera").GetComponent<Camera>();
        // Fields may switch values on iteration
        walkField = 86.39528f; // 82.5 at time of writing
        blockField = 77.79565f;
        sprintField = 91.91364f;
        attackField = 96.89904f;
        playerCam.fieldOfView = walkField;*/
        cull = transform.Find("Player Camera").GetComponent<Camera>().cullingMask;
        Debug.Log(cull);
        canStick = true;
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        selfAction = GetComponent<PlayerAction>();
        canPause = GetComponent<CanPauseGame>();
        canThrow = GetComponent<CanThrow>();
        pause_event_subscription = EventBus.Subscribe<PauseEvent>(_OnPauseChange);
        gtr = transform.Find("GroundDetector").gameObject.GetComponent<SphereCollider>().radius;
        rayDist = 0.05f;

        playerCamera = transform.Find("Player Camera").GetComponent<Camera>();
        walkFOV = playerCamera.fieldOfView;
        sprintFOV = walkFOV + 9f;
    }

    void Update()
    {
        gtl = transform.Find("GroundDetector").transform.position;
        if (Physics.SphereCast(gtl, transform.Find("GroundDetector").gameObject.GetComponent<SphereCollider>().radius,
            -Vector3.up, out hit, 0.05f + rayDist, LayerMask.GetMask("Ground")))
        {
            Debug.Log("Can Jump");
            canJump = true;
        }
        else
        {
            canJump = false;
        }
        CStamina = GetComponent<HasStamina>().getStamina();
        if (CStamina == 0f || movementInput == Vector2.zero || isSprinting == false)
        {
            isSprinting = false;
            changeSprintFOV = false;
        }
        if(changeSprintFOV == true)
        {
            float sprintTime = (Time.time - sprintStartTime) / sprintFOVDuration;
            playerCamera.fieldOfView = Mathf.SmoothStep(playerCamera.fieldOfView, sprintFOV, sprintTime);
        }
        if(changeSprintFOV == false && playerCamera.fieldOfView > walkFOV)
        {
            float sprintTime = (Time.time - sprintStartTime) / sprintFOVDuration;
            playerCamera.fieldOfView = Mathf.SmoothStep(playerCamera.fieldOfView, walkFOV, sprintTime);
        }
        if (CStamina == 0f && animator.GetBool("Block") == true)
        {
            animator.SetBool("Block", false);
            selfAction.playerAction = PlayerAction.Action.IDLE;
        }
    }

    private void FixedUpdate()
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            if (isSprinting == true)
            {
                transform.Translate(new Vector3(movementInput.x, 0, movementInput.y) * sprintSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector3(movementInput.x, 0, movementInput.y) * speed * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gtl, gtr);
        Gizmos.DrawWireSphere(new Vector3(gtl.x, gtl.y - 0.05f - rayDist, gtl.z), gtr);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (/*selfAction.playerAction != PlayerAction.Action.STUNNED &&*/ canMove)
        {
            movementInput = ctx.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove && ctx.performed)
        {
            if (canJump)
            {
                canJump = false;
                rb.velocity = new Vector3(rb.velocity.x, jump_height, rb.velocity.z);
            }
        }
    }

    // OLD VERSION. TESTING NEW IDEA
    /*public void OnPoke(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            if (ctx.performed && canStick && GetComponent<HasStamina>().getStamina() > 2.5f)
            {
                isSprinting = false;
                StartCoroutine(DisableStick(setPokeVar));
                GetComponent<HasStamina>().OnPoke();
                Debug.Log("Test Poke");
                StartCoroutine(selfAction.SetPoking(timeOnPoke, timeOfPoke)); // this input variable "timer" needs to be shorter/same length as the animation itself
                animator.SetTrigger("Poke");
            }
        }
    }  */

    //NEW VERSION
    public void OnPoke(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            if (ctx.performed && canStick && GetComponent<HasStamina>().getStamina() > 2.5f)
            {
                animator.SetTrigger("Poke");
                isSprinting = false;
                Debug.Log("Test Poke");
                selfAction.pokeEvent();
            }
        }
    }

    //OLD VERSION
    /*public void OnSwing(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            if (ctx.performed && canStick && GetComponent<HasStamina>().getStamina() > 2.5f)
            {
                isSprinting = false;
                StartCoroutine(DisableStick(setSwingVar));
                GetComponent<HasStamina>().OnSwing();
                Debug.Log("Test Swing");
                StartCoroutine(selfAction.SetSwinging(timeOnSwing, timeOfSwing)); // this input variable "timer" needs to be shorter/same length as the animation itself
                animator.SetTrigger("Swing");
            }
        }
    }    */

    // NEW VERSION
    public void OnSwing(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            if (ctx.performed && canStick && GetComponent<HasStamina>().getStamina() > 2.5f)
            {
                animator.SetTrigger("Swing");
                isSprinting = false;
                Debug.Log("Test Swing");
                selfAction.swingEvent();
            }
        }
    }

    //OLD VERSION
    /*public void OnBlock(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            Debug.Log("Test block");
            if (ctx.performed && canStick && GetComponent<HasStamina>().getStamina() > 2.5f)
            {
                isSprinting = false;
                selfAction.playerAction = PlayerAction.Action.BLOCK;
                
                if (animator.GetBool("Block") == false)
                {
                    StartCoroutine(DisableStick(setBlockVar));
                    GetComponent<HasStamina>().OnBlock();
                    Debug.Log("Switch block");
                }
                animator.SetBool("Block", true);
            }
            if (ctx.canceled || (ctx.performed && GetComponent<HasStamina>().getStamina() <= 0.05f))
            {
                selfAction.playerAction = PlayerAction.Action.IDLE;
                if (animator.GetBool("Block") == true)
                {
                    Debug.Log("Switch block");
                }
                animator.SetBool("Block", false);
            }
        }
    }*/  

    // NEW VERSION
    public void OnBlock(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            Debug.Log("Test block");
            if (ctx.performed && canStick && GetComponent<HasStamina>().getStamina() > 2.5f)
            {
                isSprinting = false;
                animator.SetBool("Block", true);
                selfAction.blockEvent();
            }
            if (ctx.canceled || (ctx.performed && GetComponent<HasStamina>().getStamina() <= 0.05f))
            {
                selfAction.playerAction = PlayerAction.Action.IDLE;
                animator.SetBool("Block", false);
            }
        }
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            if (ctx.performed && movementInput != Vector2.zero &&
                selfAction.playerAction == PlayerAction.Action.IDLE &&
                GetComponent<HasStamina>().getStamina() != 0) //toggle
            {
                Debug.Log("Sprint Pressed");
                isSprinting = !isSprinting;
                changeSprintFOV = true; // use in update 
                // flip sprint state IF Mmoving, UNLESS player 
                // attempts to start sprinting with 0 stamina
            }
        } //if the player isn't stunned, is able to move, and still has stamina
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (selfAction.playerAction != PlayerAction.Action.STUNNED && canMove)
        {
            if (ctx.performed)
            {
                Debug.Log("Test Throw");
                canThrow.ThrowObject();
            }
        }
    }

    /*
     * private void sprintFOV()
    {
        float howLong = 0.5f;

        *//*float curFOV = playerCam.fieldOfView;
        float percentChange = 0.1f;
        float valueChange = (sprintField - curFOV) * percentChange;
        while (playerCam.fieldOfView < sprintField)
        {
            StartCoroutine(changeFOV(0.05f, valueChange));
        }
    }
    private void walkFOV()
    {
        float howLong = 0.5f;
        float curFOV = playerCam.fieldOfView;
        float percentChange = 0.1f;
        float valueChange = (walkField - curFOV) * percentChange;
        while (playerCam.fieldOfView > walkField)
        {
            StartCoroutine(changeFOV(0.05f, valueChange));
        }
    }
    IEnumerator changeFOV(float duration, float from, float to)
    {
        float timer = duration;
        float timeToTake = duration;
        float val;
        while(timer >= 0)
        {
            val = Mathf.SmoothStep(from, to, timer / timeToTake);
            timer -= Time.deltaTime;
            playerCam.fieldOfView += val;
            yield return null;
        }

        yield return new WaitForSeconds(duration);
        playerCam.fieldOfView += amount;
    }
    IEnumerator DisableStick(float inputVar)
    {
        canStick = false;
        yield return new WaitForSeconds(inputVar * delayStick);
        canStick = true;
    }*/

    //NEW
    public void DisableStick()
    {
        canStick = false;
    }
    public void EnableStick()
    {
        canStick = true;
    }

    // Pause Functions
    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            canPause.PauseGame();
        }
    }

    void _OnPauseChange(PauseEvent e)
    {
        if (e.paused)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }


    public bool sprintCheck()
    {
        return isSprinting;
    }

    public float getSwingVar()
    {
        return setSwingVar;
    }

    public float getPokeVar()
    {
        return setPokeVar;
    }

    public float getBlockVar()
    {
        return setBlockVar;
    }
    public void setSprinting(bool b)
    {
        isSprinting = b;
    }
    private void OnDestroy()
    {
        EventBus.Unsubscribe(pause_event_subscription);
    }
    // Pause Functions end here

    public bool getJump()
    {
        return canJump;
    }
}



