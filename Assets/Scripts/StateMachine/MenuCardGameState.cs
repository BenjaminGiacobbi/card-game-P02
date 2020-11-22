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
    [SerializeField] Button _guideButton = null;
    [SerializeField] Text _winsText = null;
    [SerializeField] Text _lossesText = null;
    [SerializeField] Text _ratioText = null;
    [SerializeField] AudioClip _titleMusic = null;
    [SerializeField] TooltipUI _toolTips = null;
    [SerializeField] Color _selectedColor;

    Image _buttonImage = null;

    public override void Enter()
    {
        if(!_buttonImage)
        {
            _buttonImage = _guideButton.GetComponent<Image>();
            _buttonImage.color = _selectedColor;
        }
            
        _winsText.text = StateMachine.Player.Wins.ToString();
        _lossesText.text = StateMachine.Player.Losses.ToString();
        _ratioText.text = ((float)StateMachine.Player.Wins / StateMachine.Player.Losses != 0 ? StateMachine.Player.Losses : 1) + "W/L";
        _startButton.onClick.AddListener(StartGame);
        _quitButton.onClick.AddListener(QuitGame);
        _guideButton.onClick.AddListener(GuideButtonState);
        MusicController.Instance.PlayMusic(_titleMusic, 0.5f);
        EnteredMenu?.Invoke();
    }

    public override void Exit()
    {
        _startButton.onClick.RemoveListener(StartGame);
        _quitButton.onClick.RemoveListener(QuitGame);
        _guideButton.onClick.RemoveListener(GuideButtonState);
        ExitedMenu?.Invoke();
    }

    private void StartGame()
    {
        StateMachine.ChangeState<SetupCardGameState>();
    }

    private void QuitGame()
    {
        SaveSystem.SaveRecordData(StateMachine.Player.Wins, StateMachine.Player.Losses);
        Application.Quit();
    }

    private void GuideButtonState()
    {
        _toolTips.ToggleToolTips();
        if (_buttonImage.color == _selectedColor)
            _buttonImage.color = Color.white;
        else
            _buttonImage.color = _selectedColor;
    }
}
