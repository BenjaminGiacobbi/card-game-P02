using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoostStepCardGameState : CardGameState
{
    public static event Action StartedBoostStep = delegate { };
    public static event Action EndedBoostStep = delegate { };
    public static event Action PlayerBoosted = delegate { };
    public static event Action<bool> EnemyBoosted = delegate { };

    [SerializeField] Button _useBoostButton = null;
    [SerializeField] Button _skipBoostButton = null;
    [SerializeField] AudioClip _playerMusic = null;

    private void Start()
    {
        _useBoostButton.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        _useBoostButton.onClick.AddListener(PlayerBoost);
        _skipBoostButton.onClick.AddListener(EnemyBoost);
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
        _useBoostButton.onClick.RemoveListener(PlayerBoost);
        _skipBoostButton.onClick.RemoveListener(EnemyBoost);
        EndedBoostStep?.Invoke();
    }

    private void PlayerBoost()
    {
        // access player controller and tell it to use top boost card/update whatever it needs
        
        TargetController.SetTargetToPlayer();
        StateMachine.Player.BoostAction(1);
        StateMachine.Player.PlayBoostCard();
        PlayerBoosted.Invoke();
        EnemyBoost();
    }

    private void EnemyBoost()
    {
        StateMachine.Enemy.BoostAction(1);
        if (StateMachine.Enemy.BoostStepSequence())
        {
            EnemyBoosted?.Invoke(true);
        }
        else
        {
            EnemyBoosted?.Invoke(false);
            StateMachine.Enemy.BoostAction(-1);
        }
        StartCoroutine(ToTurnRoutine());
    }

    IEnumerator ToTurnRoutine()
    {
        _useBoostButton.interactable = false;
        _skipBoostButton.interactable = false;
        yield return new WaitForSeconds(0.8f);
        StateMachine.ChangeState<PlayerTurnCardGameState>();
    }
}
