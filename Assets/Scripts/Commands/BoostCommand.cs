using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostCommand : ICommand
{
    CardGameController _controller;
    ITargetable _target;

    public BoostCommand(CardGameController controller, ITargetable target)
    {
        _controller = controller;
        _target = target;
    }

    public void Execute()
    {
        TargetController.CurrentTarget = _target;
        _controller.PlayBoostCard();
    }
}
