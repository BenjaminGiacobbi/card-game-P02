using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerLoseCardGameState : CardGameState
{
    public static event Action StartedLoseState = delegate { };
    public static event Action EndedLoseState = delegate { };

    [SerializeField] Button _menuButton = null;

    public override void Enter()
    {
        StartedLoseState?.Invoke();
        _menuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public override void Exit()
    {
        EndedLoseState();
        _menuButton.onClick.RemoveListener(ReturnToMainMenu);
    }

    private void ReturnToMainMenu()
    {
        StateMachine.ChangeState<MenuCardGameState>();
    }
}
