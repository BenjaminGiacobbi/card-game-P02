using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Creature))]
public class CreatureUI : MonoBehaviour
{
    [SerializeField] Slider _hpSlider = null;
    [SerializeField] Text _hpText = null;
    [SerializeField] Text _actionText = null;
    [SerializeField] Text _defenseText = null;
    [SerializeField] Text _attackText = null;

    private Creature _attachedCreature = null;

    private void Awake()
    {
        _attachedCreature = GetComponent<Creature>();
    }

    private void OnEnable()
    {
        _attachedCreature.HealthSet += UpdateHealthDisplay;
        _attachedCreature.ActionSet += UpdateActionText;
        _attachedCreature.DefenseSet += UpdateDefenseText;
    }

    private void OnDisable()
    {
        _attachedCreature.HealthSet -= UpdateHealthDisplay;
        _attachedCreature.ActionSet -= UpdateActionText;
        _attachedCreature.DefenseSet -= UpdateDefenseText;
    }

    private void Start()
    {
        _hpSlider.minValue = 0;
        _hpSlider.maxValue = _attachedCreature.BaseHealth;
        _attackText.text = "Atk: " + _attachedCreature.AttackDamage;
    }

    private void UpdateHealthDisplay(int healthValue)
    {
        if (healthValue > _attachedCreature.BaseHealth)
            _hpSlider.maxValue = _attachedCreature.CurrentHealth;
        else
            _hpSlider.maxValue = _attachedCreature.BaseHealth;
        _hpSlider.value = healthValue;
        _hpText.text = healthValue.ToString();
    }

    private void UpdateActionText(int actionValue)
    {
        _actionText.text = "Act: " + actionValue;
    }

    private void UpdateDefenseText(float defenseValue)
    {
        _defenseText.text = "Def: " + defenseValue + "%";
    }
}
