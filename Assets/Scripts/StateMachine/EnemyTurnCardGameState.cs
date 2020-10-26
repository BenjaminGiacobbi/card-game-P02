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

    public override void Enter()
    {
        Debug.Log("Enemy Turn: ...Enter");
        EnemyTurnBegan?.Invoke();

        StartCoroutine(EnemyThinkingRoutine(_pauseDuration));
    }

    public override void Exit()
    {
        Debug.Log("Enemy Turn: Exit...");
    }

    IEnumerator EnemyThinkingRoutine(float duration)
    {
        Debug.Log("Enemy Thinking...");
        yield return new WaitForSeconds(duration);

        Debug.Log("Enemy performs action");
        EnemyTurnEnded?.Invoke();

        // turn over, return to player
        StateMachine.ChangeState<BoostStepCardGameState>();
    }
}
