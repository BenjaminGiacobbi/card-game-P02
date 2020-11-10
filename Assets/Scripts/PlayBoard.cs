using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBoard : MonoBehaviour
{
    // TODO built a more structured connection, maybe singleton?
    public static ITargetable CurrentTarget { get; set; }
    public static PlayerController CurrentPlayer { get; private set; }
    public static EnemyController CurrentEnemy { get; private set; }

    [SerializeField] PlayerController _player = null;
    [SerializeField] EnemyController _enemy = null;
    [SerializeField] SpacePair[] boardPairs = null;
    public SpacePair[] PairsArray
    { get { return boardPairs; } private set { boardPairs = value; } }


    private void Awake()
    {
        CurrentPlayer = _player;
        CurrentEnemy = _enemy;
    }


    // TODO add delays and visuals for each battle, calling upon the space/creature's own methods
    public void BattleEnemies()
    {
        foreach(SpacePair pair in PairsArray)
        {
            // damage matchups when opposing slots are filled
            if (pair.Player.Creature && pair.Enemy.Creature)
            {
                // prioritizes the presumed winning opponent, otherwise if both survive, the player goes first
                // TODO increase complexity so attacks actually happen in sequence rather than all at once, as of now Actions is just damage boosts
                int predictPlayerHP = pair.Player.Creature.CurrentHealth - 
                    pair.Enemy.Creature.AttackDamage * pair.Enemy.Creature.CurrentActions;
                int predictEnemyHP = pair.Enemy.Creature.CurrentHealth - 
                    pair.Player.Creature.AttackDamage * pair.Player.Creature.CurrentActions;

                if(predictPlayerHP > 0 || (predictPlayerHP <= 0 && predictEnemyHP <= 0))
                {
                    CurrentTarget = pair.Enemy.Creature.GetComponent<ITargetable>();
                    pair.Player.Creature.ApplyDamage(CurrentTarget);
                }
                if(predictPlayerHP <= 0 || (predictPlayerHP > 0 && predictEnemyHP > 0))
                {
                    CurrentTarget = pair.Player.Creature.GetComponent<ITargetable>();
                    pair.Enemy.Creature.ApplyDamage(CurrentTarget);
                }
            }

            // when player slot only is filled
            else if (pair.Player.Creature && !pair.Enemy.Creature)
            { 
                SetTargetToEnemy();
                pair.Player.Creature.ApplyDamage(CurrentTarget);
            }

            // when enemy slot only is filled
            else if (!pair.Player.Creature && pair.Enemy.Creature)
            {
                SetTargetToPlayer();
                pair.Enemy.Creature.ApplyDamage(CurrentTarget);
            }
        }
    }

    public void ClearBoard()
    {
        foreach (SpacePair pair in PairsArray)
        {
            pair.Player.ResetCreatureState();
            pair.Enemy.ResetCreatureState();
        }
    }


    public void SetTargetToPlayer()
    {
        CurrentTarget = CurrentPlayer.GetComponent<ITargetable>();
    }


    public void SetTargetToEnemy()
    {
        CurrentTarget = CurrentEnemy.GetComponent<ITargetable>();
    }
}

[System.Serializable]
public class SpacePair
{
    public BoardSpace Player;
    public BoardSpace Enemy;

    public SpacePair(BoardSpace player, BoardSpace enemy)
    {
        Player = player;
        Enemy = enemy;
    }
}
