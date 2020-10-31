using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActionPlayEffect", menuName = "CardData/PlayEffects/Action")]
public class ActionPlayEffect : CardPlayEffect
{
    [Header("Action value should be non-negative")]
    [SerializeField] int _actionValue = 5;

    public override void Activate(ITargetable target)
    {
        Debug.Log("Using Boost 1");
        MonoBehaviour objectToBoost = target as MonoBehaviour;
        Debug.Log(objectToBoost);
        IBoostable boostable = objectToBoost?.GetComponent<IBoostable>();
        Debug.Log(boostable);
        if (objectToBoost != null)
        {
            Debug.Log("Using Boost 2");
            if (_actionValue < 0)
                _actionValue = 0;
            boostable.BoostAction(_actionValue);
        }
    }
}
