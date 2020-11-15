using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class AbilityCardView : MonoBehaviour
{
    [SerializeField] Text _nameText = null;
    [SerializeField] Text _costText = null;
    [SerializeField] Text _descText = null;
    [SerializeField] Image _graphic = null;
    [SerializeField] Image _background = null;
    [SerializeField] Color _spawnColor;
    [SerializeField] Color _damageColor;

    public void Display(AbilityCard abilityCard)
    {
        _nameText.text = abilityCard.Name;
        _costText.text = abilityCard.Cost.ToString();
        _descText.text = abilityCard.Description;
        _graphic.sprite = abilityCard.Graphic;
        if (abilityCard.Type == AbilityType.Spawn)
            _background.color = _spawnColor;
        else if (abilityCard.Type == AbilityType.Damage)
            _background.color = _damageColor;
    }

    public void EmptyDisplay()
    {
        _nameText.text = "None";
        _costText.text = "None";
        _graphic.sprite = null;
    }
}
