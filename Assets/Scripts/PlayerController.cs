using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, IDamageable, ITargetable
{
    public event Action<Deck<AbilityCard>> CurrentHand = delegate { };
    public event Action<Deck<AbilityCard>> CurrentDiscard = delegate { };
    public event Action<Deck<AbilityCard>> CurrentMainDeck = delegate { };
    public event Action<Deck<BoostCard>> CurrentBoostDeck = delegate { };
    public event Action GameOver = delegate { };
    public event Action<int> EnergyChanged = delegate { };
    public event Action ActionEnd = delegate { };
    public event Action HealthSet = delegate { };

    public Deck<AbilityCard> _abilityDeck = new Deck<AbilityCard>();
    public Deck<AbilityCard> _abilityDiscard = new Deck<AbilityCard>();
    public Deck<AbilityCard> _playerHand = new Deck<AbilityCard>();
    public Deck<BoostCard> _boostDeck = new Deck<BoostCard>();

    [SerializeField] int _maxHandSize = 5;
    [SerializeField] float _energyModifier = 1.0f;
    [SerializeField] float _damageModifier = 1.0f;
    [SerializeField] int _playerEnergy = 10;
    [SerializeField] int _maxPlayerHealth = 30;
    private int _currentHealth;

    public int MaxHandSize
    { get { return _maxHandSize; } private set { _maxHandSize = value; } }
    
    public int PlayerEnergy
    {
        get
        {
            return _playerEnergy;
        }
        set
        {
            float dif = _playerEnergy - value;
            Debug.Log("Dif: " + Mathf.CeilToInt(dif * _energyModifier));
            _playerEnergy = _playerEnergy - Mathf.CeilToInt(dif * _energyModifier);
            if (_playerEnergy < 0)
                _playerEnergy = 0;
            EnergyChanged?.Invoke(_playerEnergy);
        }
    }
    
    public int MaxPlayerHealth
    { get { return _maxPlayerHealth; } private set { _maxPlayerHealth = value; } }

    public int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        private set
        {
            if (value > MaxPlayerHealth)
                _currentHealth = _maxPlayerHealth;
            else
                _currentHealth = value;
        }
    }

    private void Start()
    {
        CurrentHealth = MaxPlayerHealth;
    }

    public void SetupAbilityDeck(List<AbilityCardData> abilityDeckConfig)
    {
        foreach (AbilityCardData abilityData in abilityDeckConfig)
        {
            AbilityCard newAbilityCard = new AbilityCard(abilityData);
            _abilityDeck.Add(newAbilityCard);
        }

        _abilityDeck.Shuffle();
        CurrentMainDeck?.Invoke(_abilityDeck);
    }

    public void SetupBoostDeck(List<BoostCardData> boostDeckConfig)
    {
        foreach (BoostCardData boostData in boostDeckConfig)
        {
            BoostCard newBoostCard = new BoostCard(boostData);
            _boostDeck.Add(newBoostCard);
        }

        Debug.Log("test1");
        _boostDeck.Shuffle();
        CurrentBoostDeck?.Invoke(_boostDeck);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Draw();
        }
    }

    // draws a card and adds it to player hand
    private void Draw()
    {
        if (_playerHand.Count < MaxHandSize)
        {
            AbilityCard newCard = _abilityDeck.Draw(DeckPosition.Top);
            if (newCard != null)
            {
                Debug.Log("Drew card: " + newCard.Name);
                _playerHand.Add(newCard, DeckPosition.Top);

                CurrentHand?.Invoke(_playerHand);
                CurrentMainDeck?.Invoke(_abilityDeck);
            }
            if (_abilityDeck.Count == 0)
            {
                ReshuffleDiscard();
            }
        }
        else
        {
            Debug.Log("Cannot draw more than " + MaxHandSize + " cards!");
        }

    }

    public void PlayAbilityCard(int index)
    {
        AbilityCard targetCard = _playerHand.GetCard(index);
        if (targetCard != null && targetCard.Cost <= PlayerEnergy)
        {
            PlayerEnergy -= targetCard.Cost;
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
        // add event for reshuffling discard feedback?
        if (_abilityDiscard.Count > 0)
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
        if (_abilityDeck.Count > 1)
            _abilityDeck.Shuffle();

        CurrentMainDeck?.Invoke(_playerHand);
    }

    // for coordination with the state system, this would ideally be subscribed to a state change to start a turn
    public void OnTurn()
    {
        _energyModifier = 1.0f;
        _damageModifier = 1.0f;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        HealthSet?.Invoke();
        if( CurrentHealth < 0)
            Kill();
    }

    public void Kill()
    {
        CurrentHealth = 0;
        GameOver?.Invoke();
    }

    public void Target()
    {
        // some targeting code, but this would be necesssary for the AI to target
    }

    public void BoostEnergy(float modifier)
    {
        _energyModifier = modifier;
    }

    public void BoostHealth(int value)
    {
        CurrentHealth += value;
    }

    public void BoostDefense(float modifier)
    {
        _damageModifier = modifier;
    }
}
