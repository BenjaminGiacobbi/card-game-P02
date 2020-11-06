using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(CommandInvoker))]
public class EnemyBrain : MonoBehaviour
{
    [SerializeField] PlayBoard _board = null;

    [Header("Rate AI will use boost card in boost step")]
    [SerializeField] float _boostStepPercentage = 0.75f;

    [Header("Rate AI will prioritize weakened monsters with turn boost cards")]
    [SerializeField] float _boostWeakenedPriorityPercentage = 0.7f;

    [Header("Rate AI use boost on monster each action")]
    [SerializeField] float _turnBoostPercentage = 0.2f;


    EnemyController _controller = null;
    CommandInvoker _invoker = null;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _invoker = GetComponent<CommandInvoker>();
    }

    public void EnemyThinkSequence()
    {
        if (_board = null)
        {
            Debug.Log("No reference to play area. Aborting");
            return;
        }
        // if hand < max hand size and deck count > 0, draw until full

        if (true)
        {

        }

        // if actions > 0 and space is open
            
            //
    }

    public void SetTargetSpace()
    {

    }

    public void WeightedMonsterSelection() // needs a target parameter?
    {

    }
}
