using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    Subscription<PauseEvent> pause_event_subscription;

    private Rigidbody rb;
    private Vector2 cameraInput;
    public GameObject cam;

    private float ySensitivity = 100f;
    private float xSensitivity = 200f;

    private float xRotation = 0f;
    private bool canRotate = true;

    private string pc = "Player Camera";
    // Start is called before the first frame update
    void Start()
    {
        pause_event_subscription = EventBus.Subscribe<PauseEvent>(_OnPauseChange);
        cam = transform.Find(pc).gameObject;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        look();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        Debug.Log("rotated");
        cameraInput = ctx.ReadValue<Vector2>();
    }


    // Functionality found at https://github.com/lukeskt/InputSystemFirstPersonCharacter/blob/main/InputSystemFirstPersonCharacterScripts/InputSystemFirstPersonCharacter.cs
    private void look()
    {
        if(canRotate){
            float xRot = cameraInput.x * xSensitivity * Time.deltaTime;
            float yRot = cameraInput.y * ySensitivity * Time.deltaTime;

            xRotation -= yRot;
            xRotation = Mathf.Clamp(xRotation, -30f, 30f);

            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            //cam.transform.Rotate(Vector3.up * xRot);

            cam.transform.localPosition = new Vector3(0,0.65f, 0);

            transform.Rotate(Vector3.up * xRot);
        }
    }

    void _OnPauseChange(PauseEvent e)
    {
        if(e.paused)
        {
            canRotate = false;
        }
        else
        {
            canRotate = true;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(pause_event_subscription);
    }
}
