using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpace : MonoBehaviour, ITargetable
{
    [SerializeField] Transform _spawnLocation = null;
    [SerializeField] ParticleBase _spawnParticles = null;
    [SerializeField] AudioClip _failAudio = null;
    public Transform SpawnLocation { get { return _spawnLocation; } private set { _spawnLocation = value; } }
    public Creature Creature { get; set; } = null;

    public bool UseCard(AbilityCard card)
    {
        SpawnPlayEffect spawnEffect = card.PlayEffect as SpawnPlayEffect;
        if (spawnEffect != null)
        {
            if (Creature)
            {
                return false;
            }

            // this is hard to debug because card.Play()'s use of the play effect depends on CurrentTarget
            TargetController.CurrentTarget = GetComponent<ITargetable>();
            card.Play();
            return true;
        }
        else
        {
            if (!Creature)
            {
                return false;
            }
            TargetController.CurrentTarget = Creature.gameObject.GetComponent<ITargetable>();
            card.Play();
            return true;
        }
    }

    public void SpawnFeedback()
    {
        if(_spawnParticles)
            _spawnParticles.PlayComponents();
        Creature.Died += ClearCreatureState;
    }

    public void ResetCreature()
    {
        if (Creature != null)
        {
            Creature.Died -= ClearCreatureState;
            Creature.PoolReturn();
            Creature = null;
        }   
    }

    public void Target()
    {
        // target feedback
    }

    void ClearCreatureState()
    {
        Creature.Died -= ClearCreatureState;
        Creature = null;
    }
}

public enum SpaceType
{
    Player,
    Enemy
}
