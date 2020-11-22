using System.Collections;
using UnityEngine;

public class ResultsPanelUI : PanelUI
{
    [SerializeField] float _moveTime = 0.3f;
    private Vector3 _point1 = Vector3.zero;
    private Vector3 _point2 = Vector3.zero;

    private void Awake()
    {
        _point1 = new Vector3(_blockPanel.position.x + Screen.width/3, 
                             _blockPanel.position.y - Screen.height/3, 
                             _blockPanel.position.z);
        _point2 = new Vector3(_point1.x, _point1.y - Screen.height, _point1.z);
    }

    public override void OpenAnimation()
    {
        StartCoroutine(ResultsRoutine());
        _blockPanel.gameObject.SetActive(true);
    }

    public override void CloseAnimation()
    {
        LeanTween.move(_turnText.gameObject, _point2, _moveTime/2).setEaseInOutBack();
        _blockPanel.gameObject.SetActive(false);
    }

    IEnumerator ResultsRoutine()
    {
        yield return new WaitForSeconds(_moveTime);
        _turnText.text = "Results...";
        _turnText.transform.position = _point2;
        _turnText.gameObject.SetActive(true);
        LeanTween.move(_turnText.gameObject, _point1, _moveTime/2).setEaseInOutBack();
    }
}
