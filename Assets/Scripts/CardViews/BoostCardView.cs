using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BoostCardView : MonoBehaviour
{
    [SerializeField] Text _nameTextUI = null;
    [SerializeField] Text _usesTextUI = null;
    [SerializeField] Image _graphicUI = null;

    // TODO have the single card views inherit from the same base class
    public void Display(BoostCard boostCard)
    {
        _nameTextUI.text = boostCard.Name;
        _graphicUI.sprite = boostCard.Graphic;
        _usesTextUI.text = boostCard.Uses.ToString();
    }

    public void EmptyDisplay()
    {
        _nameTextUI.text = "None";
        _usesTextUI.text = "None";
        _graphicUI.sprite = null;
    }
}
