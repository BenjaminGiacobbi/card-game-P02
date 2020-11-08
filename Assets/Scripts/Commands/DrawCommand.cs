using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCommand : ICommand
{
    CardGameController _controller;

    public DrawCommand(CardGameController controller)
    {
        _controller = controller;
    }

    public void Execute()
    {
        _controller.Draw();
    }
}
