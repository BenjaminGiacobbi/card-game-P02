using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    private Queue<ICommand> CommandBuffer = new Queue<ICommand>();

    public void AddCommand(ICommand command)
    {
        CommandBuffer.Enqueue(command);
    }

    public void PlayCommands()
    {
        int count = CommandBuffer.Count;
        for(int i = 0; i < count; i++)
        {
            CommandBuffer.Dequeue().Execute();
        }
    }
}
