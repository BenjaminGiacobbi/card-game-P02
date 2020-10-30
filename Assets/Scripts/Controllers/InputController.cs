using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    // TODO definitely make this a singleton
    // TODO make this a singleton, since we'd want a single input controller for all state machines
    public event Action PressedConfirm = delegate { };
    public event Action PressedCancel = delegate { };
    public event Action PressedLeft = delegate { };
    public event Action PressedRight = delegate { };
    public event Action PressedAction = delegate { };

    void Update()
    {
        ConfirmInput();
        CancelInput();
        LeftInput();
        RightInput();
    }

    void ConfirmInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PressedConfirm?.Invoke();
    }

    void CancelInput()
    {
        // TODO probably change this from escape
        if (Input.GetKeyDown(KeyCode.Escape))
            PressedCancel?.Invoke();
    }

    void LeftInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
            PressedLeft?.Invoke();
    }

    void RightInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
            PressedRight?.Invoke();
    }

    void ActionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PressedAction?.Invoke();
        }
    }
}
