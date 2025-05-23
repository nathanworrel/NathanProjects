using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageIndicatorSystem : MonoBehaviour
{
    [Header("References")]
    public int playerNumber;
    [SerializeField] private DamageIndicator indicatorPrefab = null;
    [SerializeField] private RectTransform holder = null;
    [SerializeField] private new Camera camera = null;
    [SerializeField] private Transform player = null;

    private Dictionary<Transform, DamageIndicator> Indicators = new Dictionary<Transform, DamageIndicator>();

    #region Delegates
    public static Action<Transform> CreateIndicator = delegate { };
    public static Func<Transform, bool> CheckIfObjectInSight = null;
    #endregion

    private void Start()
    {
        if (playerNumber == 1)
        {
            player = FindObjectOfType<PlayerManager>().players[0].GetComponent<Transform>();
            camera = FindObjectOfType<PlayerManager>().players[0].GetComponentInChildren<Camera>();
        }
        else if (playerNumber == 2)
        {
            player = FindObjectOfType<PlayerManager>().players[1].GetComponent<Transform>();
            camera = FindObjectOfType<PlayerManager>().players[1].GetComponentInChildren<Camera>();
        }
    }

    private void OnEnable()
    {
        CreateIndicator += Create;
        CheckIfObjectInSight += InSight;
    }
    private void OnDisable()
    {
        CreateIndicator -= Create;
        CheckIfObjectInSight -= InSight;
    }
    void Create(Transform target)
    {
        if (Indicators.ContainsKey(target))
        {
            Indicators[target].Restart();
            return;
        }
        DamageIndicator newIndicator = Instantiate(indicatorPrefab, holder);
        newIndicator.Register(target, player, new Action( () => { Indicators.Remove(target); } ));

        Indicators.Add(target, newIndicator);
    }

    bool InSight(Transform t)
    {
        Vector3 screenPoint = camera.WorldToViewportPoint(t.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
