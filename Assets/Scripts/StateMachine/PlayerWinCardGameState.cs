using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerWinCardGameState : CardGameState
{
    public static event Action StartedWinState = delegate { };
    public static event Action EndedWinState = delegate { };

    [SerializeField] Button _menuButton = null;

    public override void Enter()
    {
        StartedWinState?.Invoke();
        StateMachine.Player.Wins++;
        _menuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public override void Exit()
    {
        EndedWinState();
        _menuButton.onClick.RemoveListener(ReturnToMainMenu);
    }

    private void ReturnToMainMenu()
    {
        StateMachine.ChangeState<MenuCardGameState>();
    }
}
