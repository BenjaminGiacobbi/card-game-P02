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
        Debug.Log("Results: ...Entering");
        StateMachine.Player.GameOver += ProceedToLose;
        // hook up enemy HP to proceed to win
        Debug.Log("Battle Enemies");
        board.BattleEnemies();
        Debug.Log("Set Timer");
        timer = 5;
    }

    public override void Tick()
    {
        Debug.Log("Tick");
        if(timer > 0)
        { 
            timer -= Time.deltaTime;
            Debug.Log("Timer: " + timer);
            if (timer <= 0)
            {
                timer = 0;
                StateMachine.ChangeState<BoostStepCardGameState>();
            }   
        }
    }

    public override void Exit()
    {
        Debug.Log("Setup: Exiting...");
        StateMachine.Player.GameOver -= ProceedToLose;
    }

    void ProceedToWin()
    {
        StateMachine.ChangeState<PlayerWinCardGameState>();
    }

    void ProceedToLose()
    {
        StateMachine.ChangeState<PlayerLoseCardGameState>();
    }
}
