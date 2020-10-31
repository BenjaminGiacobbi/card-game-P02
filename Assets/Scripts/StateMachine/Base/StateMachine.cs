using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State CurrentState => _currentState;
    protected bool InTransition { get; private set; } = false;

    private State _currentState;
    protected State _previousState;

    public void ChangeState<T>() where T : State
    {
        T targetState = GetComponent<T>();
        if (targetState == null)
        {
            Debug.LogWarning("Cannot change to a state, as it " +
                "does not exist on the State Machine Object. " +
                "Make sure you have the desired State attached " +
                "to the state Machine.");
            return;
        }
        // otherwise, found state
        InitiateStateChange(targetState);
    }

    public void RevertState()
    {
        if(_previousState != null)
        {
            InitiateStateChange(_previousState);
        }
    }

    void InitiateStateChange(State targetState)
    {
        // if your new state is different and we aren't transitioning, do it
        if (CurrentState != targetState && !InTransition)
        {
            Transition(targetState);
        }
    }

    void Transition(State newState)
    {
        InTransition = true;

        // switches state, first calling closing code on current then changing
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();

        InTransition = false;
    }

    private void Update()
    {
        // simulates update on held state with tick
        // technically this isn't necessary if the states are monobehaviors,
        // but they may not always be
        if (CurrentState != null && !InTransition)
        {
            CurrentState.Tick();
        }
    }
}
