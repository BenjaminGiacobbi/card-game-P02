using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostCard : Card
{
    public int Uses { get; private set; }
    public Sprite Graphic { get; private set; }
    public CardPlayEffect PlayEffect { get; private set; }

    public BoostCard(BoostCardData data)
    {
        Name = data.Name;
        Description = data.Description;
        Uses = data.Uses;
        Graphic = data.Graphic;
        PlayEffect = data.PlayEffect;
    }

    public override void Play()
    {
        if (Uses > 0)
        {
            PlayEffect.Activate(TargetController.CurrentTarget);
            Uses--;
        }
    }
}
