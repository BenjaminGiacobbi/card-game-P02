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
        MonoBehaviour objectToBoost = target as MonoBehaviour;
        if (objectToBoost != null)
        {
            _damageModifier = Mathf.Clamp(_damageModifier, 0.0f, 1.0f);
            objectToBoost?.GetComponent<IBoostable>().BoostDefense(_damageModifier);
        }
    }
}
