using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDefensePlayEffect", menuName = "CardData/PlayEffects/Defense")]
public class DefensePlayEffect : CardPlayEffect
{
    [Header("Modifier should be between 0 and 1")]
    [SerializeField] float _damageModifier = 0.2f;

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
            _damageModifier = Mathf.Clamp(_damageModifier, 0.0f, 1.0f);
            boostable.BoostDefense(_damageModifier);
        }
    }
}
