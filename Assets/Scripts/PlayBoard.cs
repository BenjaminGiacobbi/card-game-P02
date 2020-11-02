using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBoard : MonoBehaviour
{
    // TODO built a more structured connection, maybe singleton?
    public static ITargetable CurrentTarget { get; set; }
    public static PlayerController CurrentPlayer { get; private set; }
    public static EnemyController CurrentEnemy { get; private set; }

    [SerializeField] BoardSpace _playerSpace0 = null;
    [SerializeField] BoardSpace _playerSpace1 = null;
    [SerializeField] BoardSpace _playerSpace2 = null;
    [SerializeField] BoardSpace _enemySpace0 = null;
    [SerializeField] BoardSpace _enemySpace1 = null;
    [SerializeField] BoardSpace _enemySpace2 = null;

    SpacePair[] _pairsList = new SpacePair[3];


    private void Awake()
    {
        CurrentPlayer = FindObjectOfType<PlayerController>();
        CurrentEnemy = FindObjectOfType<EnemyController>();
    }


    // this could be made more modular in the future
    private void Start()
    {
        _pairsList[0] = new SpacePair(_playerSpace0, _enemySpace0);
        _pairsList[1] = new SpacePair(_playerSpace1, _enemySpace1);
        _pairsList[2] = new SpacePair(_playerSpace2, _enemySpace2);
    }


    // TODO add delays and visuals for each battle, calling upon the space/creature's own methods
    public void BattleEnemies()
    {
        foreach(SpacePair pair in _pairsList)
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
        foreach (SpacePair pair in _pairsList)
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

public class SpacePair
{
    public BoardSpace Player { get; private set; }
    public BoardSpace Enemy { get; private set; }

    public SpacePair(BoardSpace player, BoardSpace enemy)
    {
        Player = player;
        Enemy = enemy;
    }
}
