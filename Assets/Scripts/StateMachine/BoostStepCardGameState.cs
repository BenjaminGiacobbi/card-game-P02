using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoostStepCardGameState : CardGameState
{
    public static event Action StartedBoostStep = delegate { };
    public static event Action EndedBoostStep = delegate { };
    public static event Action BoostCardChosen = delegate { };

    [SerializeField] Button _useBoostButton = null;
    [SerializeField] Button _skipBoostButton = null;
    [SerializeField] AudioClip _playerMusic = null;

    private void Start()
    {
        _useBoostButton.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        _useBoostButton.onClick.AddListener(Boost);
        _skipBoostButton.onClick.AddListener(ToPlayerTurn);
        _useBoostButton.interactable = true;
        _skipBoostButton.interactable = true;
        _useBoostButton.gameObject.SetActive(true);
        MusicController.Instance.PlayMusic(_playerMusic, 0.5f);
        StateMachine.Player.OnTurn();
        StateMachine.Enemy.OnTurn();
        StartedBoostStep?.Invoke();
    }

    public override void Exit()
    {
        _useBoostButton.onClick.RemoveListener(Boost);
        _skipBoostButton.onClick.RemoveListener(ToPlayerTurn);
        EndedBoostStep?.Invoke();
    }

    private void Boost()
    {
        // access player controller and tell it to use top boost card/update whatever it needs
        _useBoostButton.interactable = false;
        _skipBoostButton.interactable = false;
        TargetController.SetTargetToPlayer();
        StateMachine.Player.BoostAction(1);   
        StateMachine.Player.PlayBoostCard();
        StartCoroutine(PlayerTurnRoutine());
    }

    private void ToPlayerTurn()
    {
        StateMachine.Enemy.BoostAction(1);
        StateMachine.Enemy.BoostStepSequence();
        StateMachine.ChangeState<PlayerTurnCardGameState>();
    }

    IEnumerator PlayerTurnRoutine()
    {
        BoostCardChosen.Invoke();
        yield return new WaitForSeconds(0.8f);
        ToPlayerTurn();
    }
}
