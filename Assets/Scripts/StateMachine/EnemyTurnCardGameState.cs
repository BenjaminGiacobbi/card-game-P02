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

    [SerializeField] EnemyController _enemy = null;
    [SerializeField] float _pauseDuration = 1.5f;

    public override void Enter()
    {
        EnemyTurnBegan?.Invoke();
        StartCoroutine(EnemyThinkingRoutine(_pauseDuration));
    }

    public override void Exit()
    {
        EnemyTurnEnded?.Invoke();
    }

    IEnumerator EnemyThinkingRoutine(float duration)
    {
        yield return new WaitForSeconds(duration / 2);

        _enemy.EnemyThinkSequence();
        yield return new WaitForSeconds(duration / 2);

        // turn over, return to player
        StateMachine.ChangeState<TurnResultsCardGameState>();
    }
}
