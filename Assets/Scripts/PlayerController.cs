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
    public event Action<BoostCard> SelectedBoostCard = delegate { };
    public event Action EndedSelection = delegate { };
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
    [SerializeField] int _maxHealth = 30;
    private int _currentHealth;

    Coroutine _abilityRoutine = null;
    Coroutine _boostRoutine = null;

    public int MaxHandSize
    { get { return _maxHandSize; } private set { _maxHandSize = value; } }

    public int PlayerActions
    { get { return _playerActions; } private set { _playerActions = value; } }

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

    public void SetPlayerDefaults()
    {
        CurrentHealth = _maxHealth;
        HealthSet?.Invoke(CurrentHealth);
        ClearDecks();
    }

    public void ClearDecks()
    {
        Debug.Log("Emptying Decks");
        if (!_abilityDiscard.IsEmpty)
        {
            _abilityDiscard.Empty();
            CurrentDiscard?.Invoke(_abilityDiscard);
            Debug.Log("Discard Count: " + _abilityDiscard.Count);
        }

        if (!_abilityDeck.IsEmpty)
        {
            _abilityDeck.Empty();
            CurrentMainDeck?.Invoke(_abilityDeck);
            Debug.Log("Deck Count: " + _abilityDeck.Count);
        }
    
        if (!_playerHand.IsEmpty)
        {
            _playerHand.Empty();
            CurrentHand?.Invoke(_playerHand);
            Debug.Log("Hand Count: " + _playerHand.Count);
        }

        if (!_boostDeck.IsEmpty)
        {
            _boostDeck.Empty();
            CurrentBoostDeck?.Invoke(_boostDeck);
            Debug.Log("Boost Count: " + _boostDeck.Count);
        }
            
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
        if (_boostDeck.Count > 0)
            _boostDeck.Empty();

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
        if (targetCard != null && PlayerActions > 0 && PlayerActions >= targetCard.Cost)
        {
            if (_abilityRoutine == null) 
                _abilityRoutine = StartCoroutine(AbilityCardSelection(targetCard, index));
        }
    }

    public void StartBoostRoutine()
    {
        if (_boostDeck.TopItem != null && PlayerActions > 0)
        {
            if (_boostRoutine == null)
                _boostRoutine = StartCoroutine(BoostCardSelection());
        }
    }

    public void PlayBoostCard()
    {
        BoostCard lastCard = _boostDeck.TopItem;
        PlayerActions--;
        lastCard.Play();
        // doesn't maintain boost card if it's out of uses
        if (lastCard.Uses == 0)
        {
            _boostDeck.Remove(_boostDeck.LastIndex);
        }
        else
        {
            _boostDeck.Remove(_boostDeck.LastIndex);
            _boostDeck.Add(lastCard, DeckPosition.Bottom);
        }
        ActionsChanged?.Invoke(PlayerActions);
        CurrentBoostDeck?.Invoke(_boostDeck);
        EndedSelection?.Invoke();
    }

    // TODO currently this always only iterates to half, rounded up, of the possible iterations in Discard.Count
    private void ReshuffleDiscard()
    {
        // add event for reshuffling discard feedback?
        if (_abilityDiscard.Count > 0)
        {
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
    }

    public void Target()
    {
        // some targeting feedback code, but this would be necesssary for the AI to target
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
    IEnumerator AbilityCardSelection(AbilityCard card, int index)
    {
        SelectedAbilityCard?.Invoke(card);
        yield return new WaitForEndOfFrame();
        while(true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = GetPointerRaycast();
                BoardSpace space = hit.collider?.GetComponent<BoardSpace>();
                if (space != null) // && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerSpace")
                {
                    if (!space.UseCard(card))
                    {
                        EndedSelection?.Invoke();
                        _abilityRoutine = null;
                        yield break;
                    } 
                    PlayerActions -= card.Cost;
                    _playerHand.Remove(index);
                    _abilityDiscard.Add(card);

                    ActionsChanged?.Invoke(PlayerActions);
                    CurrentDiscard?.Invoke(_abilityDiscard);
                    CurrentHand?.Invoke(_playerHand);
                    EndedSelection?.Invoke();

                    _abilityRoutine = null;
                    yield break;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                EndedSelection?.Invoke();
                _abilityRoutine = null;
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator BoostCardSelection()
    {
        SelectedBoostCard?.Invoke(_boostDeck.TopItem);
        yield return new WaitForEndOfFrame();
        while(true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = GetPointerRaycast();
                IBoostable boostable = hit.collider?.GetComponent<IBoostable>();
                if (boostable != null)
                {
                    ITargetable boostTarget = boostable as ITargetable;
                    PlayBoard.CurrentTarget = boostTarget;
                    PlayBoostCard();

                    _boostRoutine = null;
                    yield break;
                }
                else if (hit.collider?.gameObject.layer == LayerMask.NameToLayer("PlayerTarget"))
                {
                    PlayBoard.CurrentTarget = GetComponent<ITargetable>();
                    PlayBoostCard();

                    EndedSelection?.Invoke();
                    _boostRoutine = null;
                    yield break;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                EndedSelection?.Invoke();
                _boostRoutine = null;
                yield break;
            }
            yield return null;
        }
    }

    private RaycastHit GetPointerRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity)) { }
        return hitInfo;
    }
}
