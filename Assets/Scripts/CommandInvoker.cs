using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    public Stack<ICommand> CommandBuffer = new Stack<ICommand>();
    public Queue<ICommand> CommandQueue = new Queue<ICommand>();


    // direct execution
    public void ExecuteCommand(ICommand command)
    {
        CommandBuffer.Push(command);
        command.Execute();
    }

    // store
    public void AddCommandToQueue(ICommand command)
    {
        CommandQueue.Enqueue(command);
    }


    // might be useful to simplify from some methods
    public void PlayQueuedCommands()
    {
        int count = CommandQueue.Count;
        for (int i = 0; i < count; i++)
        {
            Debug.Log("Playing Command " + i);
            ICommand c = CommandQueue.Dequeue();
            c.Execute();
            CommandBuffer.Push(c);
        }
    }


    // with the public commandQueue, other scripts can time execution of commands with delays
    public void PlayTopCommand()
    {
        ICommand c = CommandQueue.Dequeue();
        c.Execute();
        CommandBuffer.Push(c);
    }
}
