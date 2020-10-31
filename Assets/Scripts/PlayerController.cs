using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, IDamageable, ITargetable, IBoostable
{
    public event Action<Deck<AbilityCard>> CurrentHand = delegate { };
    public event Action<Deck<AbilityCard>> CurrentDiscard = delegate { };
    public event Action<Deck<AbilityCard>> CurrentMainDeck = delegate { };
    public event Action<Deck<BoostCard>> CurrentBoostDeck = delegate { };
    public event Action<AbilityCard> SelectedAbilityCard = delegate { };
    public event Action EndedSelection = delegate { };
    public event Action GameOver = delegate { };
    public event Action ActionEnd = delegate { };
    public event Action<int> ActionsChanged = delegate { };
    public event Action<int> HealthSet = delegate { };
    public event Action<int> HandSizeChanged = delegate { };
    public event Action<float> DefenseChanged = delegate { };

    public Deck<AbilityCard> _abilityDeck = new Deck<AbilityCard>();
    public Deck<AbilityCard> _abilityDiscard = new Deck<AbilityCard>();
    public Deck<AbilityCard> _playerHand = new Deck<AbilityCard>();
    public Deck<BoostCard> _boostDeck = new Deck<BoostCard>();

    [SerializeField] int _maxHandSize = 10;
    [SerializeField] int _startingHandSize = 5;
    [SerializeField] float _damageModifier = 1.0f;
    [SerializeField] int _playerActions = 1;
    [SerializeField] int _maxPlayerHealth = 30;
    private int _currentHealth;

    Coroutine _selectionRoutine = null;

    public int MaxHandSize
    { get { return _maxHandSize; } private set { _maxHandSize = value; } }

    public int PlayerActions
    { get { return _playerActions; } private set { _playerActions = value; } }

    /*
    public int PlayerActions
    {
        get
        {
            return _playerActions;
        }
        private set
        {
            if (value <= 0)
                _maxHandSize = 0;
            else
                _maxHandSize = value;
        }
    }
    */

    /*
    public int MaxPlayerHealth
    { get { return _maxPlayerHealth; } private set { _maxPlayerHealth = value; } }
    */

    public int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        private set
        {
            if (value < 0)
                _currentHealth = 0;
            else
                _currentHealth = value;
        }
    }

    public int CurrentHandSize
    { get { return _startingHandSize; } private set { _startingHandSize = value; } }

    /*
    public int CurrentHandSize
    {
        get
        {
            return _startingHandSize;
        }
        private set
        {
            if (value > MaxHandSize)
                _startingHandSize = MaxHandSize;
            else
                _startingHandSize = value;
        }
    }
    */

    private void Start()
    {
        CurrentHealth = _maxPlayerHealth;
        HealthSet?.Invoke(CurrentHealth);
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

        _boostDeck.Shuffle();
        CurrentBoostDeck?.Invoke(_boostDeck);
    }

    // draws a card and adds it to player hand
    public void Draw()
    {
        if (_playerHand.Count < CurrentHandSize)
        {
            AbilityCard newCard = _abilityDeck.Draw(DeckPosition.Top);
            if (newCard != null)
            {
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
        if (targetCard != null && PlayerActions > 0)
        {
            if (_selectionRoutine == null) 
                _selectionRoutine = StartCoroutine(SelectionRaycast(targetCard, index));
        }
    }

    public void PlayTopBoostCard()
    {
        BoostCard targetCard = _boostDeck.TopItem;
        if (targetCard != null && PlayerActions > 0)
        {
            PlayerActions--;
            ActionsChanged?.Invoke(PlayerActions);
            targetCard.Play();

            // doesn't maintain boost card if it's out of uses
            if (targetCard.Uses == 0)
            {
                _boostDeck.Remove(_boostDeck.LastIndex);
            }
            else
            {
                _boostDeck.Remove(_boostDeck.LastIndex);
                _boostDeck.Add(targetCard, DeckPosition.Bottom);
            } 
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
        _damageModifier = 1.0f;
        DefenseChanged?.Invoke(_damageModifier * 100);
        PlayerActions = 1;
        ActionsChanged?.Invoke(PlayerActions);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= Mathf.CeilToInt(damage * _damageModifier);
        HealthSet?.Invoke(CurrentHealth);
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

    public void BoostHealth(int value)
    {
        CurrentHealth += value;
        HealthSet?.Invoke(CurrentHealth);
    }

    public void BoostDefense(float modifier)
    {
        _damageModifier = modifier;
        DefenseChanged?.Invoke(1.0f / _damageModifier * 100);
    }

    public void BoostAction(int value)
    {
        _playerActions += value;
        ActionsChanged?.Invoke(PlayerActions);
    }

    // TODO I don't think it's best that this goes here, but I don't know a better way to organize it yet
    // TODO these if checks are kinda disgusting
    IEnumerator SelectionRaycast(AbilityCard card, int index)
    {
        SelectedAbilityCard?.Invoke(card);
        yield return new WaitForEndOfFrame();
        while(true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit _hitInfo, Mathf.Infinity))
                {
                    BoardSpace space = _hitInfo.collider.GetComponent<BoardSpace>();
                    if (space != null)
                    {
                        if (!space.UseCard(card))
                        {
                            EndedSelection?.Invoke();
                            _selectionRoutine = null;
                            yield break;
                        } 
                        PlayerActions--;
                        ActionsChanged?.Invoke(PlayerActions);
                        _playerHand.Remove(index);
                        _abilityDiscard.Add(card);
                        EndedSelection?.Invoke();
                        CurrentDiscard?.Invoke(_abilityDiscard);
                        CurrentHand?.Invoke(_playerHand);
                        _selectionRoutine = null;
                        yield break;
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                EndedSelection?.Invoke();
                _selectionRoutine = null;
                yield break;
            }

            yield return null;
        }
    }
}
