using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SetupCardGameState : CardGameState
{
    // need a better way to do events like this, it's getting reptitive
    public static event Action StartedSetup = delegate { };
    public static event Action EndedSetup = delegate { };

    [Header("Deck Configurations")]
    [SerializeField] List<AbilityCardData> _playerAbilityDeck = new List<AbilityCardData>();
    [SerializeField] List<BoostCardData> _playerBoostDeck = new List<BoostCardData>();
    [SerializeField] List<AbilityCardData> _enemyAbilityDeck = new List<AbilityCardData>();
    [SerializeField] List<BoostCardData> _enemyBoostDeck = new List<BoostCardData>();
    [SerializeField] PlayBoard _board = null;
    [SerializeField] Button _nextButton = null;

    public override void Enter()
    {
        StartedSetup?.Invoke();
        // CANT change state while still in Enter/Exit
        // DONT put ChangeState<> here
        Debug.Log("Setup: Entering...");
        _board.ClearBoard();
        StateMachine.Player.SetDefaults();
        StateMachine.Player.SetupAbilityDeck(_playerAbilityDeck);
        StateMachine.Player.SetupBoostDeck(_playerBoostDeck);
        StateMachine.Enemy.SetDefaults();
        StateMachine.Enemy.SetupAbilityDeck(_enemyAbilityDeck);
        StateMachine.Enemy.SetupBoostDeck(_enemyBoostDeck);

        if (_nextButton != null)
            _nextButton.onClick.AddListener(ToBoostState);
    }

    public override void Tick()
    {
        // probably better to use a delay here
        if(_nextButton == null)
        {
            Debug.LogError("Setup Continue button not set. Bouncing to main menu");
            StateMachine.ChangeState<MenuCardGameState>();
        }
    }

    public override void Exit()
    {
        Debug.Log("Setup: Exiting...");
        _nextButton.onClick.RemoveListener(ToBoostState);
        EndedSetup?.Invoke();
    }

    // TODO not sure what ot name this
    private void ToBoostState()
    {
        StateMachine.ChangeState<BoostStepCardGameState>();
    }
}
