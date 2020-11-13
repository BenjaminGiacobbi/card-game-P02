using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : ICommand
{
    Creature _creature = null;
    ITargetable _target = null;

    public AttackCommand(Creature creature, ITargetable target)
    {
        _creature = creature;
        _target = target;
    }
    
    public void Execute()
    {
        _creature.ApplyDamage(_target);
    }
}
