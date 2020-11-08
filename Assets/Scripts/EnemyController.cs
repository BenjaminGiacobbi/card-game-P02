using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CommandInvoker))]
public class EnemyController : CardGameController, IDamageable, ITargetable, IBoostable
{
    [SerializeField] PlayBoard _board = null;

    [Header("Rate AI will use boost card in boost step")]
    [SerializeField] float _boostStepPercentage = 0.75f;

    [Header("Rate AI will prioritize weakened monsters with turn boost cards")]
    [SerializeField] float _boostWeakenedPriorityPercentage = 0.7f;

    [Header("Rate AI use boost on monster each action")]
    [SerializeField] float _turnBoostPercentage = 0.2f;


    [SerializeField] Slider _hpSlider = null;
    [SerializeField] Text _hpText = null;
    CommandInvoker _invoker = null;
    bool[,] _spaceMatrix = null;
    int[] _playableHand = null;
    int _actionCount = 0;

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
        Debug.Log(_board.PairsList.Length);
        _spaceMatrix = new bool[_board.PairsList.Length, 2];
        ClearSpaceMatrix();
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _hpSlider.value = CurrentHealth;
        _hpText.text = CurrentHealth.ToString();
    }


    public void RunCommandSequence()
    {
        _invoker.PlayCommands();
    }


    private bool RollChance(float percentChance)
    {
        float random = Random.Range(0.0f, 1.0f);
        Debug.Log("Random: " + random);
        Debug.Log("Test: " + (1 - percentChance));
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
        Debug.Log("[" + _spaceMatrix[0, 0] + "]   [" + _spaceMatrix[1, 0] + "]   [" + _spaceMatrix[2, 0] + "]\n" +
                  "[" + _spaceMatrix[0, 1] + "]   [" + _spaceMatrix[1, 1] + "]   [" + _spaceMatrix[2, 1] + "]");
    }


    // chance for boost card, set this object to target, and boost
    public void BoostStepSequence()
    {
        if(BoostDeck.Count > 0)
        {
            // play boost command
            if (RollChance(_boostStepPercentage))
            {
                Debug.Log(_boostStepPercentage * 100 + ", PLAY BOOST");
                PlayBoard.CurrentTarget = GetComponent<ITargetable>();
                _invoker.AddCommand(new BoostCommand(this));
                RunCommandSequence();
            }
            else
            {
                Debug.Log((1 - _boostStepPercentage) * 100 + ", SKIP BOOST");
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
            Debug.Log("DRAW SEQUENCE");
            int drawAmount = CurrentHandSize - Hand.Count;
            for(int i = 0; i < drawAmount; i++)
            {
                _invoker.AddCommand(new DrawCommand(this));
            }
            RunCommandSequence();
        }


        _actionCount = Actions;

        Debug.Log("ACTIONS: " + _actionCount);
        for (int i = 0; i < Hand.Count; i++)
        {
            Debug.Log(Hand.GetCard(i).Name);
        }

        // process sequence while actions remain
        while (_actionCount > 0)
        {
            Debug.Log("ACTION SEQUENCE: ");
            // record of costs available to play
            _playableHand = new int[Hand.Count];
            int costArrayIndex = 0;
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand.GetCard(i).Cost <= _actionCount)
                {
                    _playableHand[costArrayIndex] = i;
                    costArrayIndex++;
                }
            }

            // TODO this fails to update because actions don't run until after the sequence, so it needs to update creature count predictively
            // also fills matrix references for current board layout
            int creatureCount = CollectCreatureInfo();
            Debug.Log("CREATURE COUNT: " + creatureCount);


            // first decide whether or not to use boost card (chance on if creatures or always on MAX creatures)
            if ((creatureCount > 0 && _playableHand.Length == 0) ||
                (creatureCount > 0 && RollChance(_turnBoostPercentage)) || 
                creatureCount == _board.PairsList.Length)
            {
                // Roll Chance again to weight to focus on a low HP monster, then set as target
                // boost command with target of creature
                Debug.Log("TURN BOOST BRANCH");
                _actionCount--;
                continue;
            }
            else if (_playableHand.Length > 0 && creatureCount < _board.PairsList.Length)
            {
                Debug.Log("TURN SPAWN BRANCH");
                if (PlayInOpposedSpaces())
                    continue;
                else if (PlayInUnopposedSpaces())
                    continue;
            }

            // if it can't perform a single action up to this point,
            // pass turn
            Debug.Log("PASS TURN");
            break;
        }
        _actionCount = 0;
        _playableHand = null;
    }


    private void SetTarget(BoardSpace space)
    {
        PlayBoard.CurrentTarget = space.GetComponent<ITargetable>();
    }


    private void SetTarget(Creature creature)
    {
        PlayBoard.CurrentTarget = creature.GetComponent<ITargetable>();
    }

    private int CollectCreatureInfo()
    {
        ClearSpaceMatrix();
        int enemyCreatures = 0;
        for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
        {
            // if a creature is not null, true reference in matrix position
            Debug.Log("Column " + i + " Enemy: " + _board.PairsList[i].Enemy.Creature);
            if (_board.PairsList[i].Enemy.Creature)
            {
                _spaceMatrix[i, 0] = true;
                enemyCreatures++;
            }

            Debug.Log("Column " + i + " Player: " + _board.PairsList[i].Player.Creature);
            if (_board.PairsList[i].Player.Creature)
                _spaceMatrix[i, 1] = true;
        }
        Debug.Log("[" + _spaceMatrix[0, 0] + "]   [" + _spaceMatrix[1, 0] + "]   [" + _spaceMatrix[2, 0] + "]\n" +
                  "[" + _spaceMatrix[0, 1] + "]   [" + _spaceMatrix[1, 1] + "]   [" + _spaceMatrix[2, 1] + "]");

        return enemyCreatures;
    }


    // seeks to counter opposed spaces and plays a useful creature
    private bool PlayInOpposedSpaces()
    {
        Debug.Log("SEARCHING FOR OPPOSED SPACE");
        // get a list of indexes of opposed spaces
        List<int> indexes = new List<int>();
        for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
        {
            // check if any player spaces are filled but enemy spaces aren't
            if (!_spaceMatrix[i, 0] && _spaceMatrix[i, 1])
            {
                Debug.Log("Adding index " + i + " as opposed");
                indexes.Add(i);
            }
                
        }


        // spawn a useful creature
        if (indexes.Count > 0)
        {
            Debug.Log("FOUND OPPOSED SPACE");
            int selectedIndex = -1;
            if (indexes.Count == 1)
                selectedIndex = indexes[0];
            else
                selectedIndex = Random.Range(0, indexes.Count - 1);
            Debug.Log("Selected Index: " + indexes[selectedIndex]);
            SetTarget(_board.PairsList[indexes[selectedIndex]].Enemy);
            if (WeightedMonsterSelection())
            {
                _spaceMatrix[indexes[selectedIndex], 0] = true;
                return true;
            }
            else
                return false;
        }

        Debug.Log("FAILED TO SPAWN");
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
            {
                Debug.Log("Adding index " + i + " as unopposed");
                indexes.Add(i);
            }
                
        }

        // plays a powerful creature to damage the player a lot
        if(indexes.Count > 0)
        {
            Debug.Log("FOUND UNOPPOSED SPACE");
            Debug.Log(indexes);
            int selectedIndex = -1;
            if (indexes.Count == 1)
                selectedIndex = indexes[0];
            else
                selectedIndex = Random.Range(0, indexes.Count - 1);
            Debug.Log("Selected Index: " + indexes[selectedIndex]);
            SetTarget(_board.PairsList[indexes[selectedIndex]].Enemy);

            if (PowerMonsterSelection())
            {
                _spaceMatrix[indexes[selectedIndex], 0] = true;
                return true;
            }
            else
                return false;
        }

        Debug.Log("FAILED TO SPAWN");
        return false;
    }

    private bool WeightedMonsterSelection() // needs a target parameter?
    {
        // roll weight for full random first
        if (RollChance(0.15f))
        {
            Debug.Log("RANDOM MONSTER");
            int index = Random.Range(0, Hand.Count - 1);
            _invoker.AddCommand(new AbilityCommand(this, index));
            return true;
        }

        float highestAvg = 0;
        int avgIndex = -1;
        int highestHP = 0;
        int hpIndex = -1;

        // finds the highest avg stat and HP creature spawns among the playable hand
        for (int i = 0; i < _playableHand.Length; i++)
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

        Debug.Log("HIGHEST AVG: " + highestAvg);
        Debug.Log("HIGHEST HP: " + highestHP);

        if (RollChance(0.6f) || hpIndex == -1)
        {
            Debug.Log("SPAWN HIGHEST AVG");
            _invoker.AddCommand(new AbilityCommand(this, avgIndex));
            _actionCount -= Hand.GetCard(avgIndex).Cost;
            return true;
        }
        else if (hpIndex != -1)
        {
            Debug.Log("SPAWN HIGHEST HP");
            _invoker.AddCommand(new AbilityCommand(this, hpIndex));
            _actionCount -= Hand.GetCard(hpIndex).Cost;
            return true;
        }

        Debug.Log("FAILED TO SPAWN");
        return false;
    }


    private bool PowerMonsterSelection()
    {
        int highestAtk = 0;
        int atkIndex = -1;

        // finds the highest avg stat and HP creature spawns among the playable hand
        for (int i = 0; i < _playableHand.Length; i++)
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

        Debug.Log("HIGHEST ATK: " + highestAtk);

        if (atkIndex != -1)
        {
            Debug.Log("SPAWN POWER");
            _invoker.AddCommand(new AbilityCommand(this, atkIndex));
            _actionCount -= Hand.GetCard(atkIndex).Cost;
            return true;
        }


        Debug.Log("FAILED TO SPAWN");
        return false;
    }
}
