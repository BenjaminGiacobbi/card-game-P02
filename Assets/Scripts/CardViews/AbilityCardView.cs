using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class AbilityCardView : MonoBehaviour
{
    [SerializeField] Text _nameTextUI = null;
    [SerializeField] Text _costTextUI = null;
    [SerializeField] Image _graphicUI = null;

    public void Display(AbilityCard abilityCard)
    {
        _nameTextUI.text = abilityCard.Name;
        _costTextUI.text = abilityCard.Cost.ToString();
        _graphicUI.sprite = abilityCard.Graphic;
    }

    public void EmptyDisplay()
    {
        _nameTextUI.text = "None";
        _costTextUI.text = "None";
        _graphicUI.sprite = null;
    }
}
