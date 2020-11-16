using System.Collections;
using UnityEngine;
using System;

public class EnemyTurnCardGameState : CardGameState
{
    // TODO note that static events are dangerous if you have multiple objects
    // works well for state hookups because you generally don't have multiple
    // but be aware
    public static event Action EnemyTurnBegan = delegate { };
    public static event Action EnemyTurnEnded = delegate { };

    [SerializeField] float _pauseDuration = 1.5f;
    [SerializeField] AudioClip _enemyMusic = null;

    public override void Enter()
    {
        EnemyTurnBegan?.Invoke();
        MusicController.Instance.PlayMusic(_enemyMusic, 0.5f);
        StateMachine.Enemy.EnemyThinkSequence();
    }

    private void OnEnable()
    {
        StateMachine.Enemy.EndedSequence += ToResults;
    }

    private void OnDisable()
    {
        StateMachine.Enemy.EndedSequence -= ToResults;
    }

    public override void Exit()
    {
        EnemyTurnEnded?.Invoke();
    }

    private void ToResults()
    {
        StateMachine.ChangeState<TurnResultsCardGameState>();
    }
}
