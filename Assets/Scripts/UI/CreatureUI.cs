using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Creature))]
public class CreatureUI : MonoBehaviour
{
    [SerializeField] GameObject _uiGroup = null;
    [SerializeField] ParticleBase _attackParticles = null;
    [SerializeField] string _boostParticleTag = "BoostParticles";
    [SerializeField] string _destroyParticleTag = "DestroyParticles";
    [SerializeField] Color _boostHPColor;
    [SerializeField] Color _boostActColor;
    [SerializeField] Color _boostDefColor;
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
    }

    private void OnEnable()
    {
        _attachedCreature.HealthSet += UpdateHealthDisplay;
        _attachedCreature.ActionSet += UpdateActionText;
        _attachedCreature.DefenseSet += UpdateDefenseText;
        _attachedCreature.Spawned += CalibrateUI;
        _attachedCreature.Attack += AttackAnimation;
        _attachedCreature.Boosted += BoostFeedback;
        _attachedCreature.Died += DeathFeedback;
    }

    private void OnDisable()
    {
        _attachedCreature.HealthSet -= UpdateHealthDisplay;
        _attachedCreature.ActionSet -= UpdateActionText;
        _attachedCreature.DefenseSet -= UpdateDefenseText;
        _attachedCreature.Spawned -= CalibrateUI;
        _attachedCreature.Attack -= AttackAnimation;
        _attachedCreature.Boosted -= BoostFeedback;
        _attachedCreature.Died -= DeathFeedback;
    }

    private void Start()
    {
        _hpSlider.minValue = 0;
        _startingPosition = transform.position;
        _attackText.text = "Atk: " + _attachedCreature.AttackDamage;
    }

    private void CalibrateUI()
    {
        _hpSlider.maxValue = _attachedCreature.BaseHealth;
        _uiGroup.transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
        _uiGroup.transform.Rotate(new Vector3(0, 180, 0));
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

    private void BoostFeedback(string boostedString)
    {
        GameObject boost = ObjectPooler.Instance.SpawnObject(_boostParticleTag, null, transform.position, transform.rotation);
        LeanTween.delayedCall(0.5f, () => { ObjectPooler.Instance.ReturnToPool(_boostParticleTag, boost); });
        ParticleBase particles = boost.GetComponent<ParticleBase>();
        switch (boostedString)
        {
            case "HP":
                particles.ChangeColor(_boostHPColor);
                break;
            case "Act":
                particles.ChangeColor(_boostActColor);
                break;
            case "Def":
                particles.ChangeColor(_boostDefColor);
                break;
            default:
                break;
        }
        particles.PlayComponents();
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
        GameObject destroy = ObjectPooler.Instance.SpawnObject(_destroyParticleTag, null, transform.position, transform.rotation);
        LeanTween.delayedCall(0.5f, () => { ObjectPooler.Instance.ReturnToPool(_destroyParticleTag, destroy); });
        destroy.GetComponent<ParticleBase>().PlayComponents();
    }
}
