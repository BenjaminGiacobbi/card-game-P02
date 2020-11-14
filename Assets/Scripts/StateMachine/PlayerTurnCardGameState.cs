using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerTurnCardGameState : CardGameState
{
    public static event Action StartedPlayerTurn = delegate { };
    public static event Action EndedPlayerTurn = delegate { };

    [SerializeField] Button _nextButton = null;

    public override void Enter()
    {
        _nextButton.onClick.AddListener(OnPressedConfirm);
        StartedPlayerTurn?.Invoke();
    }

    public override void Exit()
    {
        _nextButton.onClick.RemoveAllListeners();
        EndedPlayerTurn?.Invoke();
    }

    void OnPressedConfirm()
    {
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
