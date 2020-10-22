using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoostStepCardGameState : CardGameState
{
    public static event Action StartedBoostStep = delegate { };
    public static event Action EndedBoostStep = delegate { };

    [SerializeField] DeckTester _deckTester = null;
    [SerializeField] Button _useBoostButton = null;
    [SerializeField] Button _skipBoostButton = null;

    public override void Enter()
    {
        _useBoostButton.onClick.AddListener(PlayBoostCard);
        _skipBoostButton.onClick.AddListener(ToPlayerTurn);
        Debug.Log("Boost: ...Entering");
        StartedBoostStep?.Invoke();
    }

    public override void Exit()
    {
        Debug.Log("Boost: Exiting...");
        EndedBoostStep?.Invoke();
    }

    private void PlayBoostCard()
    {
        // access player controller and tell it to use top boost card/update whatever it needs
        _deckTester.PlayTopBoostCard();
        ToPlayerTurn();
    }

    private void ToPlayerTurn()
    {
        StateMachine.ChangeState<PlayerTurnCardGameState>();
    }
}
