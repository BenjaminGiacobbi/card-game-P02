using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnResultsCardGameState : CardGameState
{
    [SerializeField] PlayBoard board = null;
    private float timer = 0;
    // receives input from the board's command invoker?

    // allows commands to play

    // exits upon commands finish (no player input in this state, just showing battle results

    // short delay() between calling command execute

    public override void Enter()
    {
        board.BattleEnemies();
        timer = 5;
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
        Debug.Log("Setup: Exiting...");
    }
}
