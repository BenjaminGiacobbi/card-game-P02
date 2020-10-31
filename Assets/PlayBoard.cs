﻿using System.Collections;
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

    public void BattleEnemies()
    {
        Debug.Log("Battling");
        foreach(SpacePair pair in _pairsList)
        {
            // damage matchups when opposing slots are filled
            if (pair.PlayerSpace.Creature && pair.EnemySpace.Creature)
            {
                // prioritizes the presumed winning opponent, otherwise if both survive, the player goes first
                int predictPlayerHP = pair.PlayerSpace.Creature.CurrentHealth - pair.EnemySpace.Creature.AttackDamage;
                int predictEnemyHP = pair.EnemySpace.Creature.CurrentHealth - pair.PlayerSpace.Creature.AttackDamage;

                if(predictPlayerHP > 0)
                {
                    CurrentTarget = pair.EnemySpace.Creature.GetComponent<ITargetable>();
                    pair.PlayerSpace.Creature.ApplyDamage(CurrentTarget);
                }
                if(predictPlayerHP == 0 || (predictPlayerHP > 0 && predictEnemyHP > 0))
                {
                    CurrentTarget = pair.PlayerSpace.Creature.GetComponent<ITargetable>();
                    pair.EnemySpace.Creature.ApplyDamage(CurrentTarget);
                }
            }

            // when player slot only is filled
            else if (pair.PlayerSpace.Creature && !pair.EnemySpace.Creature)
            {
                Debug.Log("Only Player");
                SetTargetToEnemy();
                pair.PlayerSpace.Creature.ApplyDamage(CurrentTarget);
            }

            // when enemy slot only is filled
            else if (!pair.PlayerSpace.Creature && pair.EnemySpace.Creature)
            {
                Debug.Log("Only Enemy");
                SetTargetToPlayer();
                pair.EnemySpace.Creature.ApplyDamage(CurrentTarget);
            }
        }
        Debug.Log("Finished battling");
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
    public BoardSpace PlayerSpace { get; private set; }
    public BoardSpace EnemySpace { get; private set; }

    public SpacePair(BoardSpace player, BoardSpace enemy)
    {
        PlayerSpace = player;
        EnemySpace = enemy;
    }
}
