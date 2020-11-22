using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelUI : PanelUI
{
    [SerializeField] float _moveTime = 0.5f;
    private Vector3 _point = Vector3.zero;

    private void Awake()
    {
        _point = new Vector3(_blockPanel.position.x, _blockPanel.position.y - Screen.height, _blockPanel.position.z);
    }

    public override void OpenAnimation()
    {
        _blockPanel.gameObject.SetActive(true);
        gameObject.SetActive(true);
        StartCoroutine(MainPanelRoutine());
    }

    public override void CloseAnimation()
    {
        _turnText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private IEnumerator MainPanelRoutine()
    {
        yield return new WaitForSeconds(_moveTime / 2);
        _turnText.text = "Player Step";
        _turnText.transform.position = _point;
        _turnText.gameObject.SetActive(true);
        LeanTween.move(_turnText.gameObject, _blockPanel.transform.position, _moveTime).setEaseInOutBack();

        yield return new WaitForSeconds(_moveTime * 2);
        LeanTween.move(_turnText.gameObject, _point, _moveTime/2).setOnComplete(
                () => { _blockPanel.gameObject.SetActive(false); });
    }
}
