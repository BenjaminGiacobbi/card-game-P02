using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCard : Card
{
    public int Cost { get; private set; }
    public Sprite Graphic { get; private set; }
    public CardPlayEffect PlayEffect { get; private set; }

    public AbilityCard(AbilityCardData data)
    {
        Name = data.Name;
        Cost = data.Cost;
        Graphic = data.Graphic;
        PlayEffect = data.PlayEffect;
    }

    public override void Play()
    {
        Debug.Log("Playing " + Name + " on target.");
        PlayEffect.Activate(PlayBoard.CurrentTarget);
    }
}
