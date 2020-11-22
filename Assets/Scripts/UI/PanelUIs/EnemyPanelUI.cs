using UnityEngine;
using UnityEngine.UI;

public class EnemyPanelUI : PanelUI
{
    [SerializeField] float _moveTime = 0.3f;
    private Vector3 _point = Vector3.zero;

    private void Awake()
    {
        _point = new Vector3(_blockPanel.position.x, _blockPanel.position.y - Screen.height, _blockPanel.position.z);
    }

    public override void OpenAnimation()
    {
        _blockPanel.gameObject.SetActive(true);
        _turnText.text = "Enemy Step";
        _turnText.transform.position = _point;
        _turnText.gameObject.SetActive(true);
        LeanTween.move(_turnText.gameObject, _blockPanel.transform.position, _moveTime).setEaseInOutBack();
    }

    public override void CloseAnimation()
    {
        LeanTween.move(_turnText.gameObject, _point, _moveTime).setOnComplete(
            () => { _blockPanel.gameObject.SetActive(false); });
    }
}
