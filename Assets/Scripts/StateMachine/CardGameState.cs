using UnityEngine;

// each state needs to attach to an object with a state machine,
// therefore we can search with GetComponent in Awake
[RequireComponent(typeof(CardGameSM))]
public class CardGameState : State
{
    protected CardGameSM StateMachine { get; private set; }

    private void Awake()
    {
        StateMachine = GetComponent<CardGameSM>();
    }
}
