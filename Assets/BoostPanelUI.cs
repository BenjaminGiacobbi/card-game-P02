﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoostDeckView))]
public class BoostPanelUI : PanelUI
{
    [SerializeField] RectTransform _boostCard = null;
    [SerializeField] RectTransform _targetPanel = null;
    [SerializeField] RectTransform _blockPanel = null;
    [SerializeField] RectTransform _useButton = null;
    [SerializeField] RectTransform _skipButton = null;
    [SerializeField] Text _turnText = null;
    [SerializeField] float _moveTime = 0.35f;

    BoostDeckView _deckView = null;
    private Vector3 _point1 = Vector3.zero;
    private Vector3 _point2 = Vector3.zero;

    private void Awake()
    {
        _deckView = GetComponent<BoostDeckView>();
        _point1 = new Vector3(_blockPanel.position.x, _blockPanel.position.y + _blockPanel.rect.height, _blockPanel.position.z);
        _point2 = new Vector4(_blockPanel.position.x, _blockPanel.position.y - _blockPanel.rect.height, _blockPanel.position.z);
    }

    public override void OpenAnimation()
    {
        _boostCard.gameObject.SetActive(false);
        _skipButton.gameObject.SetActive(false);
        _useButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        StartCoroutine(OpenRoutine());
    }

    public override void CloseAnimation()
    {
        _skipButton.gameObject.SetActive(false);
        _useButton.gameObject.SetActive(false);
        LeanTween.delayedCall(_deckView.AnimationTime, MoveCard);
    }

    private void MoveCard()
    {
        Vector3 startPos = _boostCard.position;
        LeanTween.move(_boostCard.gameObject, _targetPanel.position, _moveTime).setOnComplete(
            () => { FinishMoveCard(startPos); });
    }

    private void FinishMoveCard(Vector3 position)
    {
        gameObject.SetActive(false);
        _boostCard.position = position;

    }

    IEnumerator OpenRoutine()
    {
        _blockPanel.gameObject.SetActive(true);
        _turnText.transform.position = _point2;
        _turnText.text = "Boost Step";
        _turnText.gameObject.SetActive(true);
        LeanTween.move(_turnText.gameObject, _blockPanel.transform.position, 0.5f);
        yield return new WaitForSeconds(1f);

        LeanTween.move(_turnText.gameObject, _point1, 0.5f);
        yield return new WaitForSeconds(0.5f);

        _turnText.gameObject.SetActive(false);
        _blockPanel.gameObject.SetActive(false);

        PopIn(_boostCard, _moveTime / 3);
        PopIn(_useButton, _moveTime / 3);
        PopIn(_skipButton, _moveTime / 3);
        
        _boostCard.gameObject.SetActive(true);
        yield break;
    }

    private void PopIn(RectTransform rect, float popTime)
    {
        Vector2 scale = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
        rect.sizeDelta = new Vector2(0, 0);
        rect.gameObject.SetActive(true);
        LeanTween.size(rect, scale, popTime).setEaseInOutBounce();
    }
}
