using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class TitleScreenController : MonoBehaviour
{   
    bool opt_in = false;
    
    public void PlayGame()
    {
        AnalyticsService.Instance.ProvideOptInConsent(consentIdentifier, opt_in);
        EventBus.Publish<FadeEvent>(new FadeEvent(2, 0, 1));
        StartCoroutine(FadeToGame());
    }

    public IEnumerator FadeToGame(){
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        AnalyticsService.Instance.ProvideOptInConsent(consentIdentifier, opt_in);
        Application.Quit();
    }

    public void OptIn(){
        opt_in = !opt_in;
        AnalyticsService.Instance.ProvideOptInConsent(consentIdentifier, opt_in);
    }

    string consentIdentifier;
    bool isOptInConsentRequired;
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
            if (consentIdentifiers.Count > 0)
            {
                consentIdentifier = consentIdentifiers[0];
                isOptInConsentRequired = consentIdentifier == "pipl";
            }
        }
        catch (ConsentCheckException e)
        {
          // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
          Debug.Log(e.Reason);
        }
    }

}
