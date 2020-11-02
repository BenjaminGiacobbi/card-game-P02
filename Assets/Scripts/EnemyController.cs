using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, ITargetable, IDamageable, IBoostable
{
    // it might be worth it to just have enemy controller inherit from player controller, or just have a SECOND playerController that the command
    // invoker interfaces with
    [SerializeField] Slider _hpSlider = null;
    [SerializeField] Text _hpText = null;
    [SerializeField] int _maxHealth = 30;
    private int _currentHealth;

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

    public void SetDefaults()
    {
        CurrentHealth = _maxHealth;
        _hpSlider.minValue = 0;
        _hpSlider.maxValue = _maxHealth;
        _hpSlider.value = CurrentHealth;
        _hpText.text = "Enemy HP: " + CurrentHealth;
    }

    public void BoostAction(int value) { }

    public void BoostDefense(float modifier) { }

    public void BoostHealth(int value) { }

    public void Kill()
    {
        CurrentHealth = 0;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= Mathf.CeilToInt(damage * 1);   // hardcoded, add damage modifier later
        Debug.Log(CurrentHealth);
        _hpSlider.value = CurrentHealth;
        _hpText.text = "Enemy HP: " + CurrentHealth;
        if (CurrentHealth < 0)
        {
            Kill();
        }   
    }

    public void Target()
    {
        // targeting code etc
    }

}
