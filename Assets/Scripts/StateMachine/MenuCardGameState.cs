using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuCardGameState : CardGameState
{
    public static event Action EnteredMenu = delegate { };
    public static event Action ExitedMenu = delegate { };

    [SerializeField] Button _startButton = null;
    [SerializeField] Button _quitButton = null;

    public override void Enter()
    {
        Debug.Log("Menu: Entering...");
        _startButton.onClick.AddListener(StartGame);
        _quitButton.onClick.AddListener(QuitGame);
        EnteredMenu?.Invoke();
    }

    public override void Exit()
    {
        _startButton.onClick.RemoveListener(StartGame);
        _quitButton.onClick.RemoveListener(QuitGame);
        ExitedMenu?.Invoke();
        Debug.Log("Menu: Exiting...");
    }

    public void StartGame()
    {
        StateMachine.ChangeState<SetupCardGameState>();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
