using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCommand : ICommand
{
    ITargetable _target;
    CardGameController _controller;
    int _handIndex = 0;

    public AbilityCommand(CardGameController controller, ITargetable target, int index)
    {
        _controller = controller;
        _target = target;
        _handIndex = index;
    }

    public void Execute()
    {
        PlayBoard.CurrentTarget = _target;
        _controller.PlayAbilityCard(_handIndex);
    }
}
