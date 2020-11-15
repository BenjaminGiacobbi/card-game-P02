using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoostCardView))]
public class EnemyBoostCard : MonoBehaviour
{
    [SerializeField] CardGameController _controller = null;
    private BoostCardView _view = null;

    private void Awake()
    {
        _view = GetComponent<BoostCardView>();
    }

    private void OnEnable()
    {
        _controller.CurrentBoostDeck += ShowBoostCard;
    }

    private void OnDisable()
    {
        _controller.CurrentBoostDeck -= ShowBoostCard;
    }

    private void ShowBoostCard(Deck<BoostCard> deck)
    {
        _view.Display(deck.TopItem);
    }
}
