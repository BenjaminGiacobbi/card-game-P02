using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour, ITargetable, IDamageable, IBoostable
{
    [SerializeField] Slider _hpSlider = null;

    [SerializeField] int _currentHealth = 10;
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

    public BoardSpace CurrentSpace { get; private set; }

    private void Start()
    {
        _hpSlider.minValue = 0;
        _hpSlider.maxValue = CurrentHealth;   // TODO this needs to be more flexible, it's just set rn
        _hpSlider.value = CurrentHealth;
    }

    public void Kill()
    {
        CurrentSpace.Creature = null;
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= Mathf.CeilToInt(damage * DefenseModifier);
        _hpSlider.value = CurrentHealth;
        _hpSlider.value = CurrentHealth;
        if (_currentHealth <= 0)
            Kill();
    }

    public void ApplyDamage(ITargetable target)
    {
        MonoBehaviour targetObject = target as MonoBehaviour;
        IDamageable damageable = targetObject?.GetComponent<IDamageable>();
        damageable?.TakeDamage(AttackDamage);

        // play visual/audio feedback
    }

    public void SetSpace(BoardSpace space)
    {
        CurrentSpace = space;
    }

    public void Target()
    {
        Debug.Log("Creature has been targeted.");
    }

    public void BoostHealth(int value)
    {
        CurrentHealth += value;
    }

    public void BoostAction(int value)
    {
        CurrentActions += value;
    }

    public void BoostDefense(float modifier)
    {
        DefenseModifier = modifier;
    }
}
