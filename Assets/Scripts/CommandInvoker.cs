using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    private Stack<ICommand> CommandBuffer = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        CommandBuffer.Push(command);
        command.Execute();
    }
}
