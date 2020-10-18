using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, IDamageable, ITargetable
{
    public event Action GameOver = delegate { };
    public event Action<int> EnergyChanged = delegate { };
    public event Action ActionEnd = delegate { };

    [SerializeField] int _maxHandSize = 5;
    public int MaxHandSize
    { get { return _maxHandSize; } private set { _maxHandSize = value; } }

    [SerializeField] int _playerEnergy = 10;
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

    [SerializeField] int _maxPlayerHealth = 30;
    public int MaxPlayerHealth
    { get { return _maxPlayerHealth; } private set { _maxPlayerHealth = value; } }

    private int _currentHealth;
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

    [SerializeField] float _energyModifier = 1.0f;
    [SerializeField] float _damageModifier = 1.0f;

    private void Start()
    {
        CurrentHealth = MaxPlayerHealth;
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
