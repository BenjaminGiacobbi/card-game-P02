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
    bool[,] _spaceMatrix;

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

        _spaceMatrix = new bool[_board.PairsList.Length, 2];
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
        float testValue = 1f - percentChance;
        if (random >= percentChance)
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
            if(RollChance(_boostStepPercentage))
            {
                PlayBoard.CurrentTarget = GetComponent<ITargetable>();
                // play boost command
            }
            RunCommandSequence();
        }
    }


    // TODO the enemy can't process damage effects at this point and it'd cause it to skip the turn
    public void EnemyThinkSequence()
    {
        // protection for this sequence
        if (_board = null)
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
                // draw command
            }
            RunCommandSequence();
        }


        // process sequence while actions remain
        while (Actions > 0)
        {
            ClearSpaceMatrix();

            // record of costs available to play
            int playableCards = 0;
            int[] handCosts = new int[Hand.Count];
            for (int i = 0; i < Hand.Count; i++)
            {
                handCosts[i] = Hand.GetCard(i).Cost;
                if (handCosts[i] <= Actions)
                    playableCards++;
            }


            // also fills matrix references for current board layout
            int creatureCount = CollectCreatureInfo();


            // first decide whether or not to use boost card (chance on if creatures or always on MAX creatures)
            if ((creatureCount > 0 && playableCards == 0) ||
                (creatureCount > 0 && RollChance(_turnBoostPercentage)) || 
                creatureCount == _board.PairsList.Length)
            {
                // Roll Chance again to weight to focus on a low HP monster, then set as target
                // boost command with target of creature
                continue;
            }
            else if (creatureCount > 0 && creatureCount <= _board.PairsList.Length)
            {
                if (PlayInOpposedSpaces())
                    continue;
                if (PlayInUnopposedSpaces())
                    continue;
            }

            // if it can't perform a single action up to this point,
            // pass turn
            break;
        }
    }


    private void SetTarget(BoardSpace space)
    {
        PlayBoard.CurrentTarget = space.GetComponent<ITargetable>();
    }


    private void SetTarget(Creature creature)
    {
        PlayBoard.CurrentTarget = creature.GetComponent<ITargetable>();
    }


    private void WeightedMonsterSelection() // needs a target parameter?
    {
        // roll weight for highest avg, highest hp, or full random
        if (RollChance(0.15f))
        {
            int index = Random.Range(0, Hand.Count - 1);
            // add spawn command using index
            return;
        }

        Creature highestAvg = new Creature(0, 0);
        int avgIndex = 0;
        Creature highestHP = new Creature(0, 0);
        int hpIndex = 0;

        // finds the highest avg stat and HP creature spawns among the playable hand
        for(int i = 0; i < Hand.Count; i++)
        {
            SpawnPlayEffect spawn = Hand.GetCard(i).PlayEffect as SpawnPlayEffect;
            if (spawn != null)
            {
                if ((spawn.Creature.BaseHealth + spawn.Creature.AttackDamage) / 2 >= (highestAvg.BaseHealth + highestAvg.AttackDamage) / 2)
                {
                    highestAvg = spawn.Creature;
                    avgIndex = i;
                }
                if (spawn.Creature.BaseHealth >= highestHP.BaseHealth)
                {
                    highestHP = spawn.Creature;
                    hpIndex = i;
                }
            }
        }

        if (RollChance(0.6f))
        {
            // spawn command using avgIndex
        }
        else
        {
            // spawn command using hpIndex
        }
    }


    private void PowerMonsterSelection()
    {
        Creature highestAtk = new Creature(0, 0);
        int atkIndex = 0;

        // finds the highest avg stat and HP creature spawns among the playable hand
        for (int i = 0; i < Hand.Count; i++)
        {
            SpawnPlayEffect spawn = Hand.GetCard(i).PlayEffect as SpawnPlayEffect;
            if (spawn != null)
            {
                if (spawn.Creature.AttackDamage >= highestAtk.AttackDamage)
                {
                    highestAtk = spawn.Creature;
                    atkIndex = i;
                }
            }
        }

        // spawn command using atkIndex
    }


    private int CollectCreatureInfo()
    {
        int enemyCreatures = 0;
        foreach (SpacePair pair in _board.PairsList)
        {
            for (int i = 0; i < _spaceMatrix.GetLength(0); i++)
            {
                // if a creature is not null, true reference in matrix position
                if (pair.Enemy.Creature)
                {
                    _spaceMatrix[i, 0] = true;
                    enemyCreatures++;
                }
                if (pair.Player.Creature)
                    _spaceMatrix[i, 1] = true;
            }
        }

        return enemyCreatures;
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
            int selectedIndex = Random.Range(0, indexes.Count - 1);
            SetTarget(_board.PairsList[selectedIndex].Enemy);
            WeightedMonsterSelection();
            return true;
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
            int selectedIndex = Random.Range(0, indexes.Count - 1);
            SetTarget(_board.PairsList[selectedIndex].Enemy);
            PowerMonsterSelection();
            return true;
        }

        return false;
    }
}
