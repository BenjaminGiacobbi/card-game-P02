using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckTester : MonoBehaviour
{
    public event Action<Deck<AbilityCard>> CurrentHand = delegate { };
    public event Action<Deck<AbilityCard>> CurrentDiscard = delegate { };
    public event Action<Deck<AbilityCard>> CurrentMainDeck = delegate { };

    [SerializeField] List<AbilityCardData> _abilityDeckConfig = new List<AbilityCardData>();
    [SerializeField] AbilityCardView _abilityCardView = null;
    [SerializeField] int _maxHandSize = 5;
    public Deck<AbilityCard> _abilityDeck = new Deck<AbilityCard>();
    public Deck<AbilityCard> _abilityDiscard = new Deck<AbilityCard>();
    public Deck<AbilityCard> _playerHand = new Deck<AbilityCard>();

    void Start()
    {
        SetupAbilityDeck();

        // Create some new cards
        
        // Draw a new card from the deck

        // play the new card
    }

    private void SetupAbilityDeck()
    {
        foreach(AbilityCardData abilityData in _abilityDeckConfig)
        {
            AbilityCard newAbilityCard = new AbilityCard(abilityData);
            _abilityDeck.Add(newAbilityCard);
        }

        _abilityDeck.Shuffle();
        CurrentMainDeck?.Invoke(_abilityDeck);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Draw();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PrintPlayerHand();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintDeck(_playerHand);
            PrintDeck(_abilityDeck);
            PrintDeck(_abilityDiscard);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayTopCard();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReshuffleDiscard();
        }
    }

    private void Draw()
    {
        if(_playerHand.Count < _maxHandSize)
        {
            AbilityCard newCard = _abilityDeck.Draw(DeckPosition.Top);
            if (newCard != null)
            {
                Debug.Log("Drew card: " + newCard.Name);
                _playerHand.Add(newCard, DeckPosition.Top);

                CurrentHand?.Invoke(_playerHand);
                CurrentMainDeck?.Invoke(_abilityDeck);
            }
            if(_abilityDeck.Count == 0)
            {
                ReshuffleDiscard();
            }
        }
        else
        {
            Debug.Log("Cannot draw more than " + _maxHandSize + " cards!");
        }
        
    }

    private void PrintPlayerHand()
    {
        for (int i = 0; i < _playerHand.Count; i++)
        {
            Debug.Log("Player Hand Card: " + _playerHand.GetCard(i).Name);
        }
    }

    private void PrintDeck(Deck<AbilityCard> deckToPrint)
    {
        string printString = "";
        for (int i = 0; i < deckToPrint.Count; i++)
        {
            printString += deckToPrint.GetCard(i).Name;
            if (i != deckToPrint.Count - 1)
                printString += ", ";
        }

        Debug.Log(deckToPrint.ToString() + ": " + printString);
    }

    private void PlayTopCard()
    {
        AbilityCard targetCard = _playerHand.TopItem;
        if (targetCard != null)
        {
            targetCard.Play();
            // TODO expand remove to accept a deck position
            _playerHand.Remove(_playerHand.LastIndex);
            _abilityDiscard.Add(targetCard);
            CurrentDiscard?.Invoke(_abilityDiscard);
            CurrentHand?.Invoke(_playerHand);
            Debug.Log("Card added to Discard: " + targetCard.Name);
        }
    }

    // TODO currently this always only iterates to half, rounded up, of the possible iterations in Discard.Count
    private void ReshuffleDiscard()
    {
        Debug.Log("Discard count: " + _abilityDiscard.Count);
        if(_abilityDiscard.Count > 0)
        {
            for(int i = 0; i < _abilityDiscard.Count; i++)
            {
                // TODO might do something more here with the Add(List<T> function)
                AbilityCard transferCard = _abilityDiscard.Draw();
                Debug.Log(transferCard);
                _abilityDeck.Add(transferCard);
            }
            CurrentDiscard?.Invoke(_abilityDiscard);
        }

        // shuffle ability deck only if it contains more than 1 item
        if(_abilityDeck.Count > 1)
            _abilityDeck.Shuffle();

        CurrentMainDeck?.Invoke(_playerHand);
    }
}
