using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCard : Card
{
    public int Cost { get; private set; }
    public Sprite Graphic { get; private set; }
    public CardPlayEffect PlayEffect { get; private set; }
    public AbilityType Type { get; private set; }

    public AbilityCard(AbilityCardData data)
    {
        Name = data.Name;
        Cost = data.Cost;
        Graphic = data.Graphic;
        PlayEffect = data.PlayEffect;
        Type = data.Type;
    }

    public override void Play()
    {
        PlayEffect.Activate(TargetController.CurrentTarget);
    }
}
