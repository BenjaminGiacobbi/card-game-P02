using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpace : MonoBehaviour, ITargetable
{
    [SerializeField] Transform _spawnLocation = null;
    public Transform SpawnLocation { get { return _spawnLocation; } private set { _spawnLocation = value; } }
    public Creature Creature { get; set; } = null;

    private void Start()
    {
        // _spawnParticles.transform.position = new Vector3(_spawnLocation.position.x, _spawnLocation.position.y + 0.05f, _spawnLocation.position.z);
    }

    public bool UseCard(AbilityCard card)
    {
        SpawnPlayEffect spawnEffect = card.PlayEffect as SpawnPlayEffect;
        if (spawnEffect != null)
        {
            if (Creature)
            {
                // feedback for failing to place card
                // if (_failAudio)
                    // AudioHelper.PlayClip2D(_failAudio, 0.7f);
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
        // target feedback
    }
}

public enum SpaceType
{
    Player,
    Enemy
}
