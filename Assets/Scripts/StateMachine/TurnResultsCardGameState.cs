using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnResultsCardGameState : CardGameState
{
    [SerializeField] PlayBoard _board = null;
    [SerializeField] CommandInvoker _boardInvoker = null;
    [SerializeField] float _delayTime = 2f;
    [SerializeField] Text _resultsText = null;
    private float timer = 0;

    private void Start()
    {
        _resultsText.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        // some code to move into this state
        _resultsText.text = "Results...";
        _resultsText.gameObject.SetActive(true);
        StartCoroutine(BattleRoutine());
    }

    IEnumerator BattleRoutine()
    {
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
            }
            else if (StateMachine.Player.CurrentHealth <= 0)
            {
                StateMachine.ChangeState<PlayerLoseCardGameState>();
                _boardInvoker.CommandQueue.Clear();
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
        _resultsText.gameObject.SetActive(false);
    }
}
