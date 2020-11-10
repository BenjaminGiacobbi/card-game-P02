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
        MonoBehaviour objectToBoost = target as MonoBehaviour;
        IBoostable boostable = objectToBoost?.GetComponent<IBoostable>();
        if (objectToBoost != null)
        {
            if (_healValue < 0)
                _healValue = 0;
            boostable.BoostHealth(_healValue);
        }
    }
}
