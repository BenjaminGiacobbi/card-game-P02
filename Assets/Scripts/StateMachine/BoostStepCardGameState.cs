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
    private int counter = 0;

    private void Start()
    {
        _useBoostButton.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        _useBoostButton.onClick.AddListener(Boost);
        _skipBoostButton.onClick.AddListener(ToPlayerTurn);
        _useBoostButton.gameObject.SetActive(true);
        _board.SetTargetToPlayer();
        StateMachine.Player.OnTurn();
        counter = 0;
        Debug.Log("Boost: ...Entering");
        // prompt the enemy controller to select a boost card and hold it, it'll only show to the player simultaneously after they've selected as well
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
        counter++;
        // access player controller and tell it to use top boost card/update whatever it needs
        StateMachine.Player.BoostAction(1);     // provides a free action for the boost card
        StateMachine.Player.PlayBoostCard();
        ToPlayerTurn(); // this actually needs to cycle through boost cards automatically
        _board.SetTargetToEnemy();
    }

    private void ToPlayerTurn()
    {
        StateMachine.ChangeState<PlayerTurnCardGameState>();
    }
}
