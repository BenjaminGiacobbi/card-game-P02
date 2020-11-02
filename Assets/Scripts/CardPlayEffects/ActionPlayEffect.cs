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
        MonoBehaviour objectToBoost = target as MonoBehaviour;
        IBoostable boostable = objectToBoost?.GetComponent<IBoostable>();
        if (objectToBoost != null)
        {
            if (_actionValue < 0)
                _actionValue = 0;
            boostable.BoostAction(_actionValue);
        }
    }
}
