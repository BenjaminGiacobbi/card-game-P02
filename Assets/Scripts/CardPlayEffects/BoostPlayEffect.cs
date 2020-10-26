﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoostPlayEffect", menuName = "CardData/PlayEffects/Boost")]
public class BoostPlayEffect : CardPlayEffect
{
    [SerializeField] BoostType _boostType = BoostType.Health;

    [Header("Cost and Defense values between 0 and 1. Health a positive value.")]
    [SerializeField] float _boostValue = 5f;

    public override void Activate(ITargetable target)
    {
        PlayerController player = target as PlayerController;
        if (player != null)
        {
            switch (_boostType)
            {
                case BoostType.Health:
                    player.BoostHealth(Mathf.CeilToInt(_boostValue));
                    break;
                case BoostType.Action:
                    player.BoostAction(Mathf.CeilToInt(_boostValue));
                    break;
                case BoostType.Defense:
                    player.BoostDefense(_boostValue);
                    break;
                default:
                    break;
            }
        }
        else
            Debug.Log("Error. Target is not a player.");
    }
}

public enum BoostType
{
    Health,
    Defense,
    Action
}
