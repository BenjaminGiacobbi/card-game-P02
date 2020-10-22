using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckTester : MonoBehaviour
{
    // TODO put this in a player action script that contains player decks and actions for other scripts to reference

    public event Action<Deck<AbilityCard>> CurrentHand = delegate { };
    public event Action<Deck<AbilityCard>> CurrentDiscard = delegate { };
    public event Action<Deck<AbilityCard>> CurrentMainDeck = delegate { };
    public event Action<Deck<BoostCard>> CurrentBoostDeck = delegate { };

    [Header("Deck Configurations")]
    [SerializeField] List<AbilityCardData> _abilityDeckConfig = new List<AbilityCardData>();
    [SerializeField] List<BoostCardData> _boostDeckConfig = new List<BoostCardData>();
    [SerializeField] PlayerController _player = null;
    public Deck<AbilityCard> _abilityDeck = new Deck<AbilityCard>();
    public Deck<AbilityCard> _abilityDiscard = new Deck<AbilityCard>();
    public Deck<AbilityCard> _playerHand = new Deck<AbilityCard>();
    public Deck<BoostCard> _boostDeck = new Deck<BoostCard>();

    public void SetupAbilityDeck()
    {
        foreach(AbilityCardData abilityData in _abilityDeckConfig)
        {
            AbilityCard newAbilityCard = new AbilityCard(abilityData);
            _abilityDeck.Add(newAbilityCard);
        }

        _abilityDeck.Shuffle();
        CurrentMainDeck?.Invoke(_abilityDeck);
    }

    public void SetupBoostDeck()
    {
        foreach(BoostCardData boostData in _boostDeckConfig)
        {
            BoostCard newBoostCard = new BoostCard(boostData);
            _boostDeck.Add(newBoostCard);
        }

        _boostDeck.Shuffle();
        CurrentBoostDeck?.Invoke(_boostDeck);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Draw();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayTopAbilityCard();
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            PlayTopBoostCard();
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            ChangeTurn();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReshuffleDiscard();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            AddEnergy();
        }
    }

    // draws a card and adds it to player hand
    private void Draw()
    {
        if(_playerHand.Count < _player.MaxHandSize)
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
            Debug.Log("Cannot draw more than " + _player.MaxHandSize + " cards!");
        }
        
    }

    // prints the contents of the deck, mostly for debug purposes
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

    // adds energy for testing purposes
    private void AddEnergy()
    {
        _player.PlayerEnergy++;
        if(_player.PlayerEnergy > 99)
        {
            Debug.Log("Maximum energy!");
            _player.PlayerEnergy = 99;
        }
    }

    public void PlayTopAbilityCard()
    {
        AbilityCard targetCard = _playerHand.TopItem;
        if (targetCard != null && targetCard.Cost <= _player.PlayerEnergy)
        {
            _player.PlayerEnergy -= targetCard.Cost;
            targetCard.Play();
            // TODO expand remove to accept a deck position
            _playerHand.Remove(_playerHand.LastIndex);
            _abilityDiscard.Add(targetCard);
            CurrentDiscard?.Invoke(_abilityDiscard);
            CurrentHand?.Invoke(_playerHand);
            Debug.Log("Card added to Discard: " + targetCard.Name);
        }
    }

    public void PlayTopBoostCard()
    {
        BoostCard targetCard = _boostDeck.Draw();
        if (targetCard != null)
        {
            targetCard.Play();

            // doesn't maintain boost card if it's out of uses
            if (targetCard.Uses == 0)
                return;
            else
                _boostDeck.Add(targetCard, DeckPosition.Bottom);
            CurrentBoostDeck?.Invoke(_boostDeck);
        }
    }

    // TODO currently this always only iterates to half, rounded up, of the possible iterations in Discard.Count
    private void ReshuffleDiscard()
    {
        
        if(_abilityDiscard.Count > 0)
        {
            Debug.Log("Reshuffling Discard Pile into Main Deck!");
            int _discardCount = _abilityDiscard.Count;
            for (int i = 0; i < _discardCount; i++)
            {
                // TODO might do something more here with the Add(List<T> function)
                _abilityDeck.Add(_abilityDiscard.Draw());
            }
            CurrentDiscard?.Invoke(_abilityDiscard);
        }

        // shuffle ability deck only if it contains more than 1 item
        if(_abilityDeck.Count > 1)
            _abilityDeck.Shuffle();

        CurrentMainDeck?.Invoke(_playerHand);
    }

    private void ChangeTurn()
    {
        Debug.Log("Resetting Boosts!");
        _player.OnTurn();
    }
}
