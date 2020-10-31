using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealPlayEffect", menuName = "CardData/PlayEffects/Heal")]
public class HealPlayEffect : CardPlayEffect
{
    [Header("Health value should be non-negative - negative count as damage")]
    [SerializeField] int _healValue = 5;

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
            if (_healValue < 0)
                _healValue = 0;
            boostable.BoostHealth(_healValue);
        }
    }
}
