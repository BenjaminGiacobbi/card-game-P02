using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpace : MonoBehaviour, ITargetable
{
    [SerializeField] Transform _spawnLocation = null;
    public Transform SpawnLocation { get { return _spawnLocation; } private set { _spawnLocation = value; } }
    public Creature Creature { get; set; } = null;

    public bool UseCard(AbilityCard card)
    {
        SpawnPlayEffect spawnEffect = card.PlayEffect as SpawnPlayEffect;
        if (spawnEffect != null)
        {
            if (Creature)
            {
                // feedback for vailing to place card
                return false;
            }

            // this is hard to debug because card.Play()'s use of the play effect depends on CurrentTarget
            PlayBoard.CurrentTarget = GetComponent<ITargetable>();
            card.Play();
            return true;
        }
        else
        {
            if (!Creature)
            {
                return false;
            }
            PlayBoard.CurrentTarget = Creature.gameObject.GetComponent<ITargetable>();
            card.Play();
            return true;
        }
    }

    public void ResetCreatureState()
    {
        if (Creature != null)
        {
            Creature.Kill();
            Creature = null;
        }
            
    }

    public void Target()
    {
        Debug.Log("Board Space Target");
    }
}

public enum SpaceType
{
    Player,
    Enemy
}
