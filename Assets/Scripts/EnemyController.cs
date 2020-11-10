using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CommandInvoker))]
public class EnemyController : CardGameController, IDamageable, ITargetable, IBoostable
{
    [SerializeField] PlayBoard _board = null;

    [Header("Rate AI will use boost card in boost step")]
    [SerializeField] float _boostStepPercent = 0.75f;

    [Header("Rate AI use boost on monster each action")]
    [SerializeField] float _turnBoostPercent = 0.2f;

    [Header("Rate AI will prioritize weakened monsters with turn boost cards")]
    [SerializeField] float _prioritizeWeakenedPercent = 0.7f;

    [Header("Rate AI will counter player's creatures each action")]
    [SerializeField] float _playOpposedPercent = 0.65f;

    [Header("Rate AI will prioritize monsters with high average stats")]
    [SerializeField] float _playWeightedPercent = 0.60f;

    [SerializeField] Slider _hpSlider = null;
    [SerializeField] Text _hpText = null;
    CommandInvoker _invoker = null;
    bool[,] _spaceMatrix = null;
    List<int> _playableHand = null;

    private void Awake()
    {
        _invoker = GetComponent<CommandInvoker>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _hpSlider.minValue = 0;
        _hpSlider.maxValue = _maxHealth;
        _hpSlider.value = CurrentHealth;
        _hpText.text = CurrentHealth.ToString();
        _spaceMatrix = new bool[_board.PairsArray.Length, 2];
        ClearSpaceMatrix();
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _hpSlider.value = CurrentHealth;
        _hpText.text = CurrentHealth.ToString();
    }

    public override void BoostHealth(int value)
    {
        base.BoostHealth(value);
        if (CurrentHealth > _maxHealth)
            _hpSlider.maxValue = CurrentHealth;
        else
            _hpSlider.maxValue = _maxHealth;
        _hpSlider.value = CurrentHealth;
    }

    private bool RollChance(float percentChance)
    {
        float random = Random.Range(0.0f, 1.0f);
        if (random >= (1 - percentChance))
            return true;
        else
            return false;
    }

    private void ClearSpaceMatrix()
    {
        for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _spaceMatrix.GetLength(1); j++)
            {
                _spaceMatrix[i, j] = false;
            }
        }
    }


    // chance for boost card, set this object to target, and boost
    public void BoostStepSequence()
    {
        if(BoostDeck.Count > 0)
        {
            // play boost command
            if (RollChance(_boostStepPercent))
            {
                _invoker.ExecuteCommand(new BoostCommand(this, GetComponent<ITargetable>()));
            }
        }
    }


    // TODO the enemy can't process damage effects at this point and it'd cause it to skip the turn if they were the only cost options
    public void EnemyThinkSequence()
    {
        // protection for this sequence
        if (_board == null)
        {
            Debug.LogWarning("No reference to play area. Aborting");
            return;
        }

        // draws to fill hand, and command invoker play all before moving on because enemy needs full hand to make decision
        // (also could have draw visual effect)
        if (Hand.Count < CurrentHandSize)
        {
            int drawAmount = CurrentHandSize - Hand.Count;
            for(int i = 0; i < drawAmount; i++)
            {
                _invoker.ExecuteCommand(new DrawCommand(this));
            }
        }
        
        // process sequence while actions remain
        while (Actions > 0)
        {
            // info collection for playable hand before each action
            _playableHand = new List<int>();
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand.GetCard(i).Cost <= Actions)
                    _playableHand.Add(i); 
            }

            // info collection for board state at beginning of enemy turn
            int creatureCount = CollectCreatureInfo();

            // first decide whether or not to use boost card (chance on if creatures or always on MAX creatures)
            if ((creatureCount > 0 && _playableHand.Count == 0) ||
                (creatureCount > 0 && RollChance(_turnBoostPercent)) || 
                creatureCount == _board.PairsArray.Length)
            {
                PlayRandomBoost();
                continue;
            }
            else if (_playableHand.Count > 0 && creatureCount < _board.PairsArray.Length)
            {
                // rolls chance to prioritize playing opposed, but if it fails to summon it'do the opposite
                if (RollChance(_playOpposedPercent))
                {
                    if (PlayInOpposedSpaces())
                        continue;
                    else if (PlayInUnopposedSpaces())
                        continue;
                }
                else
                {
                    if (PlayInUnopposedSpaces())
                        continue;
                    else if (PlayInOpposedSpaces())
                        continue;
                }
            }

            // if it can't perform a single action up to this point,
            // pass turn
            break;
        }
    }

    private int CollectCreatureInfo()
    {
        ClearSpaceMatrix();
        int creatures = 0;
        for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
        {
            // if a creature is not null, true reference in matrix position
            if (_board.PairsArray[i].Enemy.Creature)
            {
                _spaceMatrix[i, 0] = true;
                creatures++;
            }
            if (_board.PairsArray[i].Player.Creature)
                _spaceMatrix[i, 1] = true;
        }

        return creatures;
    }


    // should only be used during turn since it's dependant on current board
    private void PlayRandomBoost()
    {
        List<int> creatureIndexes = new List<int>();
        for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
        {
            if(_spaceMatrix[i, 0])
                creatureIndexes.Add(i);
        }

        int index = Random.Range(0, creatureIndexes.Count > 0 ? creatureIndexes.Count - 1 : 0);
        ITargetable boostTarget = _board.PairsArray[creatureIndexes[index]].Enemy.Creature.GetComponent<ITargetable>();
        _invoker.ExecuteCommand(new BoostCommand(this, boostTarget));

    }

    // seeks to counter opposed spaces and plays a useful creature
    private bool PlayInOpposedSpaces()
    {
        // get a list of indexes of opposed spaces
        List<int> indexes = new List<int>();
        for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
        {
            // check if any player spaces are filled but enemy spaces aren't
            if (!_spaceMatrix[i, 0] && _spaceMatrix[i, 1])
                indexes.Add(i);  
        }


        // spawn a useful creature
        if (indexes.Count > 0)
        {
            int selectedIndex = Random.Range(0, indexes.Count > 0 ? indexes.Count - 1 : 0);
            ITargetable spawnTarget = _board.PairsArray[indexes[selectedIndex]].Enemy.GetComponent<ITargetable>();
            return WeightedMonsterSelection(spawnTarget);
        }

        return false;
    }


    // searches for spaces without an opposed monster and plays a powerful creature
    private bool PlayInUnopposedSpaces()
    {
        // get a list of indexes of unopposed spaces
        List<int> indexes = new List<int>();
        for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
        {
            // check if any pairs are empty
            if (!_spaceMatrix[i, 0] && !_spaceMatrix[i, 1])
                indexes.Add(i);
        }

        // plays a powerful creature to damage the player a lot
        if(indexes.Count > 0)
        {
            int selectedIndex = Random.Range(0, indexes.Count > 0 ? indexes.Count - 1 : 0);
            ITargetable spawnTarget = _board.PairsArray[indexes[selectedIndex]].Enemy.GetComponent<ITargetable>();
            return PowerMonsterSelection(spawnTarget);
        }

        return false;
    }

    private bool WeightedMonsterSelection(ITargetable target) // needs a target parameter?
    {
        // roll weight for full random first
        if (RollChance(0.15f))
        {
            int index = Random.Range(0, _playableHand.Count - 1);
            _invoker.ExecuteCommand(new AbilityCommand(this, target, _playableHand[index]));
            return true;
        }

        float highestAvg = 0;
        int avgIndex = -1;
        int highestHP = 0;
        int hpIndex = -1;

        // finds the highest avg stat and HP creature spawns among the playable hand
        for (int i = 0; i < _playableHand.Count; i++)
        {
            // only searches through playable cards within current cost count
            SpawnPlayEffect spawn = Hand.GetCard(_playableHand[i]).PlayEffect as SpawnPlayEffect;
            if (spawn != null)
            {
                float creatureAvg = (spawn.Creature.BaseHealth + spawn.Creature.AttackDamage) / 2;
                if (creatureAvg >= highestAvg)
                {
                    highestAvg = creatureAvg;
                    avgIndex = _playableHand[i];
                }

                if (spawn.Creature.BaseHealth >= highestHP)
                {
                    highestHP = spawn.Creature.BaseHealth;
                    hpIndex = _playableHand[i];
                }
            }
        }

        if (RollChance(0.6f) || hpIndex == -1)
        {
            _invoker.ExecuteCommand(new AbilityCommand(this, target, avgIndex));
            return true;
        }
        else if (hpIndex != -1)
        {
            _invoker.ExecuteCommand(new AbilityCommand(this, target, hpIndex));
            return true;
        }

        return false;
    }


    private bool PowerMonsterSelection(ITargetable target)
    {
        int highestAtk = 0;
        int atkIndex = -1;

        // finds the highest avg stat and HP creature spawns among the playable hand
        for (int i = 0; i < _playableHand.Count; i++)
        {
            SpawnPlayEffect spawn = Hand.GetCard(_playableHand[i]).PlayEffect as SpawnPlayEffect;
            if (spawn != null)
            {
                if (spawn.Creature.AttackDamage >= highestAtk)
                {
                    highestAtk = spawn.Creature.AttackDamage;
                    atkIndex = _playableHand[i];
                }
            }
        }

        if (atkIndex != -1)
        {
            _invoker.ExecuteCommand(new AbilityCommand(this, target, atkIndex));
            return true;
        }

        return false;
    }
}
