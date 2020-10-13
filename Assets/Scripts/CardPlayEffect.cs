using UnityEngine;

public abstract class CardPlayEffect : ScriptableObject
{
    public abstract void Activate(ITargetable target);
}
