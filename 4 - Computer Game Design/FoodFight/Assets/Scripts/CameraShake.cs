using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform camTransform = default;
    Vector3 originalPosOfCam = default;
    public float shakeFreq = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        originalPosOfCam = camTransform.position;
    }

    private void Update()
    {
        
    }
    public IEnumerator Shake(bool longStun)
    {
        originalPosOfCam = camTransform.position;
        int shakeFrames;
        if (longStun)
        {
            shakeFrames = 240;
        }
        else
        {
            shakeFrames = 120;
        }
        for (int i = 0; i < shakeFrames; i++)
        {
            if (i % 5 == 0)
            {
                camTransform.position = originalPosOfCam + Random.insideUnitSphere * shakeFreq;
                yield return new WaitForEndOfFrame();
            }
        }
        camTransform.position = originalPosOfCam;
    }
}
