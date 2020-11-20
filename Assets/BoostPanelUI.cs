using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoostDeckView))]
public class BoostPanelUI : PanelUI
{
    [SerializeField] RectTransform _boostCard = null;
    [SerializeField] GameObject _useButton = null;
    [SerializeField] GameObject _skipButton = null;
    [SerializeField] RectTransform _targetPanel = null;
    [SerializeField] float _moveTime = 0.8f;

    BoostDeckView _deckView = null;

    private void Awake()
    {
        _deckView = GetComponent<BoostDeckView>();
    }

    private void Start()
    {
        _useButton.SetActive(false);
        _skipButton.SetActive(false);
        gameObject.SetActive(false);
    }

    public override void OpenAnimation()
    {
        _useButton.SetActive(true);
        _skipButton.SetActive(true);
        gameObject.SetActive(true);
    }

    public override void CloseAnimation()
    {
        _skipButton.SetActive(false);
        _useButton.SetActive(false);
        LeanTween.delayedCall(_deckView.AnimationTime, MoveCard);
    }

    private void MoveCard()
    {
        Vector3 startPos = _boostCard.transform.position;
        Debug.Log(_targetPanel.transform.position);
        LeanTween.move(_boostCard, _targetPanel.transform.position, _moveTime).setOnComplete(
            () => { FinishMoveCard(startPos); });
    }

    private void FinishMoveCard(Vector3 position)
    {
        gameObject.SetActive(false);
        _boostCard.transform.position = position;
    }
}
