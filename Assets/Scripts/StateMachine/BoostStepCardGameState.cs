using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoostStepCardGameState : CardGameState
{
    public static event Action StartedBoostStep = delegate { };
    public static event Action EndedBoostStep = delegate { };

    [SerializeField] Button _useBoostButton = null;
    [SerializeField] Button _skipBoostButton = null;
    [SerializeField] PlayBoard _board = null;

    private void Start()
    {
        _useBoostButton.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        _useBoostButton.onClick.AddListener(Boost);
        _skipBoostButton.onClick.AddListener(ToPlayerTurn);
        _useBoostButton.gameObject.SetActive(true);
        StateMachine.Player.OnTurn();
        StateMachine.Enemy.OnTurn();
        Debug.Log("Boost: ...Entering");
        StartedBoostStep?.Invoke();
    }

    public override void Exit()
    {
        Debug.Log("Boost: Exiting...");
        _useBoostButton.onClick.RemoveListener(Boost);
        _skipBoostButton.onClick.RemoveListener(ToPlayerTurn);
        EndedBoostStep?.Invoke();
    }

    private void Boost()
    {
        // access player controller and tell it to use top boost card/update whatever it needs
        _board.SetTargetToPlayer();
        StateMachine.Player.BoostAction(1);   
        StateMachine.Player.PlayBoostCard();

        // this sequence sets target internally
        Debug.Log("Point 1");
        StateMachine.Enemy.BoostAction(1);
        StateMachine.Enemy.BoostStepSequence();
        Debug.Log("Point 2");
        ToPlayerTurn();
    }

    private void ToPlayerTurn()
    {
        StateMachine.ChangeState<PlayerTurnCardGameState>();
    }
}
