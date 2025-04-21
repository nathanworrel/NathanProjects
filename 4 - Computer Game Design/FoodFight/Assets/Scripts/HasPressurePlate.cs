using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HasPressurePlate : MonoBehaviour
{
    private int playerCount = 0;

    void Update()
    {
        if(playerCount >= 2)
        {
            EventBus.Publish<FadeEvent>(new FadeEvent(2, 0, 1));
            StartCoroutine(FadeToGame());
        }
    }

    public IEnumerator FadeToGame(){
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainScene");
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Player")
        {
            playerCount++;
        } 
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.tag == "Player")
        {
            playerCount--;
        } 
    }
}
