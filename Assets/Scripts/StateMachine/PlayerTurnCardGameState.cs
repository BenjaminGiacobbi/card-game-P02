using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerTurnCardGameState : CardGameState
{
    public static event Action StartedPlayerTurn = delegate { };
    public static event Action EndedPlayerTurn = delegate { };

    [SerializeField] Button _nextButton = null;
    [SerializeField] Button _winButton = null;
    [SerializeField] Button _loseButton = null;

    public override void Enter()
    {
        Debug.Log("Player Turn: ...Entering");

        _nextButton.onClick.AddListener(OnPressedConfirm);
        _winButton.onClick.AddListener(ProceedToWin);
        _loseButton.onClick.AddListener(ProceedToLose);
        StartedPlayerTurn?.Invoke();
    }

    public override void Exit()
    {
        Debug.Log("Player Turn: Exiting...");
        _nextButton.onClick.RemoveAllListeners();
        EndedPlayerTurn?.Invoke();
    }

    void OnPressedConfirm()
    {
        Debug.Log("Attempt to enter Enemy State!");
        // change to enemy turn state

        StateMachine.ChangeState<EnemyTurnCardGameState>();
    }

    void ProceedToWin()
    {
        StateMachine.ChangeState<PlayerWinCardGameState>();
    }

    void ProceedToLose()
    {
        StateMachine.ChangeState<PlayerLoseCardGameState>();
    }
}
