using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] CardGameController _controller = null;
    [SerializeField] Slider _hpSlider = null;
    [SerializeField] Color _damageColor;
    [SerializeField] Image _sliderFill = null;
    [SerializeField] Text _hpText = null;
    [SerializeField] Text _defText = null;
    [SerializeField] Text _actText = null;
    private RectTransform _fillRect = null;

    private void Awake()
    {
        _fillRect = _sliderFill.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _controller.HealthSet += UpdateHealthDisplay;
        _controller.ActionsChanged += UpdateActionsDisplay;
        _controller.DefenseChanged += UpdateDefenseDisplay;
    }

    private void OnDisable()
    {
        _controller.HealthSet += UpdateHealthDisplay;
        _controller.ActionsChanged += UpdateActionsDisplay;
        _controller.DefenseChanged += UpdateDefenseDisplay;
    }

    private void Start()
    {
        _hpSlider.minValue = 0;
        _hpSlider.maxValue = _controller.MaxHealth;
        _hpSlider.value = _controller.CurrentHealth;
        _hpText.text = _controller.CurrentHealth.ToString();
    }

    void UpdateHealthDisplay(int value)
    {
        if(value < _hpSlider.value)
        {
            Color temp = _sliderFill.color;
            LeanTween.color(_fillRect, _damageColor, 0.15f).setOnComplete(
                () => { LeanTween.color(_fillRect, temp, 0.1f); });
            _hpSlider.maxValue = _controller.MaxHealth;
        }
        else if (value > _controller.MaxHealth)
            _hpSlider.maxValue = _controller.CurrentHealth;
            
        _hpSlider.value = value;
        _hpText.text = value.ToString() + " / " + _controller.MaxHealth;
    }

    void UpdateActionsDisplay(int value)
    {
        _actText.text = "Actions: " + value;
    }

    void UpdateDefenseDisplay(float modifier)
    {
        _defText.text = "Defense: " + (1 / modifier * 100).ToString("f2") + "%";
    }
}
