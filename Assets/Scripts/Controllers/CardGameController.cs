using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardGameController : MonoBehaviour, IDamageable, ITargetable, IBoostable
{
    public virtual event Action<Deck<AbilityCard>> CurrentHand = delegate { };
    public virtual event Action<Deck<AbilityCard>> CurrentDiscard = delegate { };
    public virtual event Action<Deck<AbilityCard>> CurrentMainDeck = delegate { };
    public virtual event Action<Deck<BoostCard>> CurrentBoostDeck = delegate { };
    public virtual event Action<int> ActionsChanged = delegate { };
    public virtual event Action<int> HealthSet = delegate { };
    public virtual event Action<int> HandSizeChanged = delegate { };
    public virtual event Action<float> DefenseChanged = delegate { };

    public virtual Deck<AbilityCard> AbilityDeck { get; set; } = new Deck<AbilityCard>();
    public virtual Deck<AbilityCard> AbilityDiscard { get; set; } = new Deck<AbilityCard>();
    public virtual Deck<AbilityCard> Hand { get; set; } = new Deck<AbilityCard>();
    public virtual Deck<BoostCard> BoostDeck { get; set; } = new Deck<BoostCard>();

    [SerializeField] protected int _maxHandSize = 10;
    [SerializeField] protected int _startingHandSize = 5;
    [SerializeField] protected float _damageModifier = 1.0f;
    [SerializeField] protected int _actions = 1;
    [SerializeField] protected int _maxHealth = 30;
    protected int _currentHealth;

    public virtual int MaxHandSize
    { get { return _maxHandSize; } set { _maxHandSize = value; } }

    public virtual float DamageModifier
    { get { return _damageModifier; } set { _damageModifier = value; } }

    public virtual int Actions
    { get { return _actions; } set { _actions = value; } }

    public virtual int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            if (value < 0)
                _currentHealth = 0;
            else
                _currentHealth = value;
        }
    }

    public virtual int MaxHealth
    { get { return _maxHealth; } private set { _maxHealth = value;} }

    public virtual int CurrentHandSize
    { get { return _startingHandSize; } set { _startingHandSize = value; } }

    public virtual void SetDefaults()
    {
        CurrentHealth = _maxHealth;
        RaiseHealth(CurrentHealth);
        ClearDecks();
    }

    public virtual void ClearDecks()
    {
        if (!AbilityDiscard.IsEmpty)
        {
            AbilityDiscard.Empty();
            RaiseDiscard(AbilityDiscard);
        }

        if (!AbilityDeck.IsEmpty)
        {
            AbilityDeck.Empty();
            RaiseMain(AbilityDeck);
        }

        if (!Hand.IsEmpty)
        {
            Hand.Empty();
            RaiseHand(Hand);
        }

        if (!BoostDeck.IsEmpty)
        {
            BoostDeck.Empty();
            RaiseBoost(BoostDeck);
        }
    }


    public virtual void SetupAbilityDeck(List<AbilityCardData> abilityDeckConfig)
    {
        foreach (AbilityCardData abilityData in abilityDeckConfig)
        {
            AbilityCard newAbilityCard = new AbilityCard(abilityData);
            AbilityDeck.Add(newAbilityCard);
        }

        AbilityDeck.Shuffle();
        RaiseMain(AbilityDeck);
    }


    public virtual void SetupBoostDeck(List<BoostCardData> boostDeckConfig)
    {
        if (BoostDeck.Count > 0)
            BoostDeck.Empty();

        foreach (BoostCardData boostData in boostDeckConfig)
        {
            BoostCard newBoostCard = new BoostCard(boostData);
            BoostDeck.Add(newBoostCard);
        }

        BoostDeck.Shuffle();
        RaiseBoost(BoostDeck);
    }


    public virtual void Draw()
    {
        if (Hand.Count < CurrentHandSize)
        {
            AbilityCard newCard = AbilityDeck.Draw(DeckPosition.Top);
            if (newCard != null)
            {
                Hand.Add(newCard, DeckPosition.Top);

                RaiseHand(Hand);
                RaiseMain(AbilityDeck);
            }
            if (AbilityDeck.Count == 0)
            {
                ReshuffleDiscard();
            }
        }
        else
        {
            // draw fail feedback
        }
    }


    public virtual void PlayAbilityCard(int index)
    {
        AbilityCard targetCard = Hand.GetCard(index);
        if (targetCard != null && Actions > 0 && Actions >= targetCard.Cost)
        {
            targetCard.Play();
            Actions -= targetCard.Cost;
            Hand.Remove(index);
            AbilityDiscard.Add(targetCard);

            RaiseActions(Actions);
            RaiseDiscard(AbilityDiscard);
            RaiseHand(Hand);
        }   
    }


    public virtual void PlayBoostCard()
    {
        BoostCard lastCard = BoostDeck.TopItem;
        Actions--;
        lastCard.Play();
        // doesn't maintain boost card if it's out of uses
        if (lastCard.Uses == 0)
        {
            BoostDeck.Remove(BoostDeck.LastIndex);
        }
        else
        {
            BoostDeck.Remove(BoostDeck.LastIndex);
            BoostDeck.Add(lastCard, DeckPosition.Bottom);
        }
        RaiseActions(Actions);
        RaiseBoost(BoostDeck);
    }


    public virtual void ReshuffleDiscard()
    {
        // add event for reshuffling discard feedback?
        if (AbilityDiscard.Count > 0)
        {
            int _discardCount = AbilityDiscard.Count;
            for (int i = 0; i < _discardCount; i++)
            {
                // TODO might do something more here with the Add(List<T> function)
                AbilityDeck.Add(AbilityDiscard.Draw());
            }
            RaiseMain(AbilityDeck);
            RaiseDiscard(AbilityDiscard);
        }
        // shuffle ability deck only if it contains more than 1 item
        if (AbilityDeck.Count > 1)
            AbilityDeck.Shuffle();
    }


    // for coordination with the state system, this would ideally be subscribed to a state change to start a turn
    public virtual void OnTurn()
    {
        _damageModifier = 1.0f;
        Actions = 1;
        RaiseDefense(DamageModifier);
        RaiseActions(Actions);
    }


    public virtual void TakeDamage(int damage)
    {
        CurrentHealth -= Mathf.CeilToInt(damage * _damageModifier);
        RaiseHealth(CurrentHealth);
        if (CurrentHealth < 0)
            Kill();
    }


    public virtual void Kill()
    {
        CurrentHealth = 0;
    }


    public virtual void Target()
    {
        // some targeting feedback code, but this would be necesssary for the AI to target
    }


    public virtual void BoostHealth(int value)
    {
        CurrentHealth += value;
        RaiseHealth(CurrentHealth);
    }


    public virtual void BoostDefense(float modifier)
    {
        DamageModifier = modifier;
        RaiseDefense(DamageModifier);
    }


    public virtual void BoostAction(int value)
    {
        _actions += value;
        RaiseActions(Actions);
    }


    #region inheritance callers
    protected virtual void RaiseMain(Deck<AbilityCard> deck)
    { CurrentMainDeck?.Invoke(deck); }

    protected virtual void RaiseHand(Deck<AbilityCard> deck)
    { CurrentHand?.Invoke(deck); }

    protected virtual void RaiseDiscard(Deck<AbilityCard> deck)
    { CurrentDiscard?.Invoke(deck); }

    protected virtual void RaiseBoost(Deck<BoostCard> deck)
    { CurrentBoostDeck?.Invoke(deck); }

    protected virtual void RaiseHealth(int health)
    { HealthSet?.Invoke(health); }

    protected virtual void RaiseActions(int actions)
    { ActionsChanged?.Invoke(actions); }

    protected virtual void RaiseHandSize(int size)
    { HandSizeChanged?.Invoke(size); }

    protected virtual void RaiseDefense(float modifier)
    { DefenseChanged?.Invoke(modifier); }
    #endregion
}
