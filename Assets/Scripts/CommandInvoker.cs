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

    /*
    public void PlayCommands()
    {
        int count = CommandBuffer.Count;
        for(int i = 0; i < count; i++)
        {
            CommandBuffer.Push().Execute();
            // visual flair
        }
    }
    */
}
