using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerTurnCardGameState : CardGameState
{
    public static event Action StartedPlayerTurn = delegate { };
    public static event Action EndedPlayerTurn = delegate { };

    [SerializeField] Text _playerTurnTextUI = null;
    [SerializeField] Button _nextButton = null;
    [SerializeField] Button _winButton = null;
    [SerializeField] Button _loseButton = null;

    int _playerTurnCount = 0;

    public override void Enter()
    {
        Debug.Log("Player Turn: ...Entering");
        
        _playerTurnTextUI.gameObject.SetActive(true);

        _playerTurnCount++;
        _playerTurnTextUI.text = "Player Turn: " + _playerTurnCount;

        StateMachine.Player.GameOver += ProceedToLose;
        _nextButton.onClick.AddListener(OnPressedConfirm);
        _winButton.onClick.AddListener(ProceedToWin);
        _loseButton.onClick.AddListener(ProceedToLose);
        StartedPlayerTurn?.Invoke();
    }

    public override void Exit()
    {
        _playerTurnTextUI.gameObject.SetActive(false);
        Debug.Log("Player Turn: Exiting...");
        _nextButton.onClick.RemoveAllListeners();
        StateMachine.Player.GameOver -= ProceedToLose;
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
