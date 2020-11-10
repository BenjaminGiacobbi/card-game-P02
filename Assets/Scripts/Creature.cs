using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Creature : MonoBehaviour, ITargetable, IDamageable, IBoostable
{
    public event Action<int> BaseHealthSet = delegate { };
    public event Action<int> HealthSet = delegate { };
    public event Action<int> ActionSet = delegate { };
    public event Action<float> DefenseSet = delegate { };

    [SerializeField] int _baseHealth = 10;
    public int BaseHealth { get { return _baseHealth; } private set { _baseHealth = value; } }

    private int _currentHealth = 0;
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

    

    [SerializeField] int _currentActions = 1;
    public int CurrentActions
    { get { return _currentActions; } private set { _currentActions = value; } }

    [SerializeField] int _attackDamage = 5;
    public int AttackDamage
    { get { return _attackDamage; } private set { _attackDamage = value; } }

    public float DefenseModifier { get; private set; } = 1.0f;

    private void Start()
    {
        CurrentHealth = BaseHealth;
        HealthSet?.Invoke(CurrentHealth);
        ActionSet?.Invoke(CurrentActions);
        DefenseSet?.Invoke(DefenseModifier);
    }

    public void Kill()
    {
        // destroy feedback
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= Mathf.CeilToInt(damage * DefenseModifier);
        HealthSet?.Invoke(CurrentHealth);
        if (_currentHealth <= 0)
            Kill();
    }

    public void ApplyDamage(ITargetable target)
    {
        MonoBehaviour targetObject = target as MonoBehaviour;
        IDamageable damageable = targetObject?.GetComponent<IDamageable>();
        if (damageable != null)
        {
            for (int i = 0; i < CurrentActions; i++)
            {
                damageable.TakeDamage(AttackDamage);
                // play visual/audio feedback
            }
        }
    }

    public void Target()
    {
        // targeting feedback
    }

    public void BoostHealth(int value)
    {
        CurrentHealth += value;
        HealthSet?.Invoke(CurrentHealth);
    }

    public void BoostAction(int value)
    {
        CurrentActions += value;
        ActionSet?.Invoke(CurrentActions);
    }

    public void BoostDefense(float modifier)
    {
        DefenseModifier = modifier;
        DefenseSet?.Invoke(DefenseModifier);
    }
}
