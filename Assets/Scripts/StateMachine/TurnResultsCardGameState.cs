using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TurnResultsCardGameState : CardGameState
{
    public static event Action ResultsBegan = delegate { };
    public static event Action ResultsEnded = delegate { };

    [SerializeField] PlayBoard _board = null;
    [SerializeField] CommandInvoker _boardInvoker = null;
    [SerializeField] float _delayTime = 2f;

    public override void Enter()
    {
        // some code to move into this state
        ResultsBegan?.Invoke();
        StartCoroutine(BattleRoutine());
    }

    IEnumerator BattleRoutine()
    {
        yield return new WaitForSeconds(_delayTime);
        // populates command queue
        _board.BattleCommands();

        // cycles through queue
        int count = _boardInvoker.CommandQueue.Count;
        for (int i = 0; i < count; i ++)
        {
            _boardInvoker.PlayTopCommand();
            yield return new WaitForSeconds(_delayTime);

            if (StateMachine.Enemy.CurrentHealth <= 0)
            {
                StateMachine.ChangeState<PlayerWinCardGameState>();
                _boardInvoker.CommandQueue.Clear();
                yield break;
            }
            else if (StateMachine.Player.CurrentHealth <= 0)
            {
                StateMachine.ChangeState<PlayerLoseCardGameState>();
                _boardInvoker.CommandQueue.Clear();
                yield break;
            }
            
        }
        if (StateMachine.Player.BoostDeck.Count == 0)
            StateMachine.ChangeState<PlayerLoseCardGameState>();
        else if (StateMachine.Enemy.BoostDeck.Count == 0)
            StateMachine.ChangeState<PlayerWinCardGameState>();
        else
            StateMachine.ChangeState<BoostStepCardGameState>();
    }

    public override void Exit()
    {
        ResultsEnded?.Invoke();
    }
}
