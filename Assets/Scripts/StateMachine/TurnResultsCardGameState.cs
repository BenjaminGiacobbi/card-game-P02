using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnResultsCardGameState : CardGameState
{
    [SerializeField] PlayBoard _board = null;
    [SerializeField] float _waitTime = 4f;
    [SerializeField] Text _resultsText = null;
    private float timer = 0;
    // receives input from the board's command invoker?

    // allows commands to play

    // exits upon commands finish (no player input in this state, just showing battle results

    // short delay() between calling command execute

    private void Start()
    {
        _resultsText.gameObject.SetActive(false);
    }

    public override void Enter()
    {
        _board.BattleEnemies();
        _resultsText.text = "Results...";
        _resultsText.gameObject.SetActive(true);
        timer = _waitTime;
    }

    public override void Tick()
    {
        if(timer > 0)
        { 
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;

                if (StateMachine.Player.CurrentHealth <= 0)
                    StateMachine.ChangeState<PlayerLoseCardGameState>();
                else if (StateMachine.Enemy.CurrentHealth <= 0)
                    StateMachine.ChangeState<PlayerWinCardGameState>();
                else
                    StateMachine.ChangeState<BoostStepCardGameState>();
            }   
        }
    }

    public override void Exit()
    {
        _resultsText.gameObject.SetActive(false);
        Debug.Log("Setup: Exiting...");
    }
}
