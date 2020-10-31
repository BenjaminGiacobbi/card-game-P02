using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpace : MonoBehaviour, ITargetable
{
    [SerializeField] Transform _spawnLocation = null;
    [SerializeField] Text _cardText = null;
    public Transform SpawnLocation { get { return _spawnLocation; } private set { _spawnLocation = value; } }
    public Creature Creature { get; set; } = null;

    private void Start()
    {
        _cardText.text = "";
    }

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
            return false;
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
