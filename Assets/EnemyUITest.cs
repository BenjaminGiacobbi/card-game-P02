using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUITest : MonoBehaviour
{
    [SerializeField] CardGameController _enemy = null;
    [SerializeField] Text _hpText = null;
    [SerializeField] Text _defText = null;
    [SerializeField] Text _actText = null;

    private void OnEnable()
    {
        _enemy.HealthSet += UpdateHealthDisplay;
        _enemy.ActionsChanged += UpdateActionsDisplay;
        _enemy.DefenseChanged += UpdateDefenseDisplay;
    }

    private void OnDisable()
    {
        _enemy.HealthSet += UpdateHealthDisplay;
        _enemy.ActionsChanged += UpdateActionsDisplay;
        _enemy.DefenseChanged += UpdateDefenseDisplay;
    }

    void UpdateHealthDisplay(int value)
    {
        _hpText.text = "Health: " + value;
    }

    void UpdateActionsDisplay(int value)
    {
        _actText.text = "Actions: " + value;
    }

    void UpdateDefenseDisplay(float modifier)
    {
        _defText.text = "Defense: " + (1 / modifier * 100) + "%";
    }
}
