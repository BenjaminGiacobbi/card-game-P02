using UnityEngine;
using UnityEngine.UI;

public abstract class PanelUI : MonoBehaviour
{
    [SerializeField] protected Text _turnText = null;
    [SerializeField] protected RectTransform _blockPanel = null;

    public abstract void OpenAnimation();
    public abstract void CloseAnimation();
}
