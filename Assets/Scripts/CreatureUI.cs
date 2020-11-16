using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Creature))]
public class CreatureUI : MonoBehaviour
{
    [SerializeField] GameObject _uiGroup = null;
    [SerializeField] ParticleBase _attackParticles = null;
    [SerializeField] ParticleBase _boostParticles = null;
    [SerializeField] ParticleBase _destroyParticles = null;
    [SerializeField] Slider _hpSlider = null;
    [SerializeField] Text _hpText = null;
    [SerializeField] Text _actionText = null;
    [SerializeField] Text _defenseText = null;
    [SerializeField] Text _attackText = null;

    private Creature _attachedCreature = null;
    private Vector3 _startingPosition = Vector3.zero;

    private void Awake()
    {
        _attachedCreature = GetComponent<Creature>();
        CreateParticles(_boostParticles);
        CreateParticles(_destroyParticles);
    }

    private void OnEnable()
    {
        _attachedCreature.HealthSet += UpdateHealthDisplay;
        _attachedCreature.ActionSet += UpdateActionText;
        _attachedCreature.DefenseSet += UpdateDefenseText;
        _attachedCreature.Attack += AttackAnimation;
        _attachedCreature.Boosted += BoostFeedback;
        _attachedCreature.Died += DeathFeedback;
    }

    private void OnDisable()
    {
        _attachedCreature.HealthSet -= UpdateHealthDisplay;
        _attachedCreature.ActionSet -= UpdateActionText;
        _attachedCreature.DefenseSet -= UpdateDefenseText;
        _attachedCreature.Attack -= AttackAnimation;
        _attachedCreature.Boosted -= BoostFeedback;
        _attachedCreature.Died -= DeathFeedback;
    }

    private void Start()
    {
        _hpSlider.minValue = 0;
        _hpSlider.maxValue = _attachedCreature.BaseHealth;
        _attackText.text = "Atk: " + _attachedCreature.AttackDamage;
        _uiGroup.transform.rotation = Quaternion.Euler(_uiGroup.transform.rotation.x, 0, _uiGroup.transform.rotation.z);
        _startingPosition = transform.position;
    }

    private void CreateParticles(ParticleBase particles)
    {
        particles = Instantiate(particles, transform);
        particles.transform.position = transform.position;
    }

    private void UpdateHealthDisplay(int healthValue)
    {
        if (healthValue > _attachedCreature.BaseHealth)
            _hpSlider.maxValue = _attachedCreature.CurrentHealth;
        else
            _hpSlider.maxValue = _attachedCreature.BaseHealth;
        _hpSlider.value = healthValue;
        _hpText.text = healthValue.ToString();
        AnimateTextFlash(_hpText);
    }

    private void UpdateActionText(int actionValue)
    {
        _actionText.text = "Act: " + actionValue;
        AnimateTextFlash(_actionText);
    }

    private void UpdateDefenseText(float defenseValue)
    {
        _defenseText.text = "Def: " + ((1.0f / defenseValue) * 100) + "%";
        AnimateTextFlash(_defenseText);
    }

    private void BoostFeedback()
    {
        
        if (_boostParticles)
        {
            Debug.Log("Playing");
            _boostParticles.PlayComponents();
        }
            
    }

    private void AttackAnimation()
    {
        LeanTween.move(gameObject, (_startingPosition + (2 * transform.forward)), 0.3f).setEaseInOutBack().setOnComplete(
            () => { LeanTween.move(gameObject, _startingPosition, 0.15f); });
        if (_attackParticles != null)
            _attackParticles.PlayComponents();
    }

    private void AnimateTextFlash(Text text)
    {
        Color tempColor = text.color;
        RectTransform rect = text.GetComponent<RectTransform>();
        LeanTween.colorText(rect, Color.white, 0.1f).setOnComplete(
            () => { LeanTween.colorText(rect, tempColor, 0.1f); });
    }

    private void DeathFeedback()
    {
        if (_destroyParticles)
            _destroyParticles.PlayComponents();
    }
}
