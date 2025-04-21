using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

// code obtaineid from https://unity3d.college/2017/04/09/unity-analytics/
public class BoscoAnalytics : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

/*        LevelController.OnLevelComplete += LevelController_OnLevelComplete;
        PlayerController.OnPlayerDied += PlayerController_OnPlayerDied;*/
    }

    private void PlayerController_OnPlayerDied(Vector3 deathPosition)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Position", deathPosition);

        Analytics.CustomEvent("PLAYER_DIED", data);
    }

    private void LevelController_OnLevelComplete(string levelName)
    {
        Analytics.CustomEvent("LEVEL_COMPLETE:" + levelName);
    }

    private void OnDestroy()
    {
        Analytics.FlushEvents();
    }
}