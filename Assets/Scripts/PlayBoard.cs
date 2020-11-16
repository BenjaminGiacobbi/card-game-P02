using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CommandInvoker))]
public class PlayBoard : MonoBehaviour
{
    [SerializeField] SpacePair[] boardPairs = null;
    public SpacePair[] PairsArray
    { get { return boardPairs; } private set { boardPairs = value; } }

    private CommandInvoker _invoker = null;


    private void Awake()
    {
        _invoker = GetComponent<CommandInvoker>();
    }


    // TODO add delays and visuals for each battle, calling upon the space/creature's own methods
    public void BattleCommands()
    {
        foreach(SpacePair pair in PairsArray)
        {
            // damage matchups when opposing slots are filled
            if (pair.Player.Creature && pair.Enemy.Creature)
            {
                // gets battle information
                int predictPlayerHP = pair.Player.Creature.CurrentHealth - Mathf.CeilToInt(
                    pair.Enemy.Creature.AttackDamage * pair.Player.Creature.DefenseModifier) * pair.Enemy.Creature.CurrentActions;
                int predictEnemyHP = pair.Enemy.Creature.CurrentHealth - Mathf.CeilToInt(
                    pair.Player.Creature.AttackDamage * pair.Enemy.Creature.DefenseModifier) * pair.Player.Creature.CurrentActions;

                // sets a priority attacker
                Creature priorityAttacker = null;
                Creature secondaryAttacker = null;

                if (pair.Player.Creature.CurrentActions >= pair.Enemy.Creature.CurrentActions)
                {
                    priorityAttacker = pair.Player.Creature;
                    secondaryAttacker = pair.Enemy.Creature;
                }   
                else
                {
                    priorityAttacker = pair.Enemy.Creature;
                    secondaryAttacker = pair.Player.Creature;
                }

                // targets for simplicity
                ITargetable priorityTarget = priorityAttacker.GetComponent<ITargetable>();
                ITargetable secondaryTarget = secondaryAttacker.GetComponent<ITargetable>();
                int trackPriorityHP = priorityAttacker.CurrentHealth;
                int trackSecondaryHP = secondaryAttacker.CurrentHealth;

                // adds alternating actions for battle
                int j = 0;
                for (int i = 0; i < priorityAttacker.CurrentActions; i++)
                {
                    _invoker.AddCommandToQueue(new AttackCommand(priorityAttacker, secondaryTarget));

                    trackSecondaryHP -= Mathf.CeilToInt(priorityAttacker.AttackDamage * secondaryAttacker.DefenseModifier);
                    if (trackSecondaryHP <= 0)
                        break;

                    if(j < secondaryAttacker.CurrentActions)
                    {
                        _invoker.AddCommandToQueue(new AttackCommand(secondaryAttacker, priorityTarget));
                        trackPriorityHP -= Mathf.CeilToInt(secondaryAttacker.AttackDamage * priorityAttacker.DefenseModifier);
                        if (trackPriorityHP <= 0)
                            break;
                        j++;
                    }
                }
            }
            // when player slot only is filled
            else if (pair.Player.Creature && !pair.Enemy.Creature)
            {
                for (int i = 0; i < pair.Player.Creature.CurrentActions; i++)
                {
                    Debug.Log("Generating Command" + i);
                    _invoker.AddCommandToQueue(new AttackCommand(pair.Player.Creature, TargetController.CurrentEnemy));
                }
            }
            // when enemy slot only is filled
            else if (!pair.Player.Creature && pair.Enemy.Creature)
            {
                for (int i = 0; i < pair.Enemy.Creature.CurrentActions; i++)
                {
                    Debug.Log("Generating Command" + i);
                    _invoker.AddCommandToQueue(new AttackCommand(pair.Enemy.Creature, TargetController.CurrentPlayer));
                }
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
}

[System.Serializable]
public struct SpacePair
{
    public BoardSpace Player;
    public BoardSpace Enemy;

    public SpacePair(BoardSpace player, BoardSpace enemy)
    {
        Player = player;
        Enemy = enemy;
    }
}
