using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TooltipUI : MonoBehaviour
{
    private Text _helpText = null;
    private bool _help = true;

    private void Awake()
    {
        _helpText = GetComponent<Text>();
        _helpText.gameObject.SetActive(false);
    }

    public void ToggleToolTips()
    {
        _help = !_help;
    }

    public void ShowTooltip(string tooltip)
    {
        if(_help)
        {
            _helpText.text = tooltip;
            _helpText.gameObject.SetActive(true);
        }
        
    }

    public void HideTooltip()
    {
        if(_help)
            _helpText.gameObject.SetActive(false);
    }
}
