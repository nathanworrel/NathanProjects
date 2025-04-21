using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraZoomOnSprint : MonoBehaviour
{
    public Transform camTransform;
    public Vector3 originalPosOfCam;

    HasStamina playerStamina;

    bool canUpdateSprint;

    // Start is called before the first frame update
    void Start()
    {
        originalPosOfCam = camTransform.position;
        playerStamina = GetComponentInParent<HasStamina>();
        canUpdateSprint = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = camTransform.position;
        if (playerStamina.GetSprinting() && canUpdateSprint)
        {
            canUpdateSprint = false;
            targetPosition.z = .5f;
            StartCoroutine(ZoomOut(this.transform, targetPosition, 0.1f));
        }
        else if (!playerStamina.GetSprinting() && canUpdateSprint)
        {
            canUpdateSprint=false;
            targetPosition.z = 0f;
            StartCoroutine(ZoomIn(this.transform, targetPosition, 0.1f));
        }
    }

    IEnumerator ZoomOut(Transform target, Vector3 dest_pos, float duration)
    {
        float init_time = Time.time;
        float progress = (Time.time - init_time) / duration;

        while (progress < 1.0f)
        {
            progress = (Time.time - init_time) / duration;
            Vector3 new_position = Vector3.Lerp(target.position, dest_pos, progress);

            yield return null;
        }
        canUpdateSprint = true;
    }

    IEnumerator ZoomIn(Transform target, Vector3 dest_pos, float duration)
    {
        float init_time = Time.time;
        float progress = (Time.time - init_time) / duration;

        while (progress < 1.0f)
        {
            progress = (Time.time - init_time) / duration;
            Vector3 new_position = Vector3.Lerp(target.position, dest_pos, progress);

            yield return null;
        }
        canUpdateSprint = true;
    }

    /*    public void SprintZoom(bool sprinting)
        {

            Debug.Log("SPRINTZOOM2");
            if (sprinting)
            {

                Debug.Log("SPRINTZOOM3");
                originalPosOfCam.z = -.5f;
            }
            else
            {

                Debug.Log("SPRINTZOOM4");
                originalPosOfCam.z = 0;
            }
        }

        public void ZoomOut()
        {
            originalPosOfCam.z -= .5f;
        }

        public void ZoomIn()
        {
            originalPosOfCam.z += 0.5f;
        }*/
}
