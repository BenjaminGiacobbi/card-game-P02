using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostCommand : ICommand
{
    CardGameController _controller;

    public BoostCommand(CardGameController controller)
    {
        _controller = controller;
    }

    public void Execute()
    {
        _controller.PlayBoostCard();
    }
}
