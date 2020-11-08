using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCommand : ICommand
{
    CardGameController _controller;
    int _handIndex = 0;

    public AbilityCommand(CardGameController controller, int index)
    {
        _controller = controller;
        _handIndex = index;
    }

    public void Execute()
    {
        Debug.Log(_handIndex);
        _controller.PlayAbilityCard(_handIndex);
    }
}
